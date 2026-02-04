using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Commands.GenerateSubscriptionInvoice;

public class GenerateSubscriptionInvoiceCommandHandler
    : IRequestHandler<GenerateSubscriptionInvoiceCommand, InvoiceDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly ITaxService _taxService;

    public GenerateSubscriptionInvoiceCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        ITaxService taxService)
    {
        _context = context;
        _currentUser = currentUser;
        _taxService = taxService;
    }

    public async Task<InvoiceDto> Handle(
        GenerateSubscriptionInvoiceCommand request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.Deliveries.Where(d => d.Status == DeliveryStatus.Delivered && d.InvoiceId == null))
                .ThenInclude(d => d.Items)
                    .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.SubscriptionId);

        // Verify vendor owns this subscription
        if (subscription.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only generate invoices for your own subscriptions");

        // Get unbilled deliveries
        var unbilledDeliveries = subscription.Deliveries
            .Where(d => d.Status == DeliveryStatus.Delivered && d.InvoiceId == null)
            .ToList();

        if (!unbilledDeliveries.Any())
            throw new BadRequestException("No unbilled deliveries found for this subscription");

        // Calculate invoice period
        var periodStart = unbilledDeliveries.Min(d => d.ScheduledDate);
        var periodEnd = unbilledDeliveries.Max(d => d.ScheduledDate);

        // Calculate totals
        var subtotal = unbilledDeliveries.Sum(d => d.TotalAmount);
        var tax = _taxService.CalculateTax(subtotal, 0); // No discount for subscriptions
        var total = subtotal + tax;

        // Generate invoice number
        var invoiceNumber = await GenerateInvoiceNumberAsync(ct);

        var invoice = new SubscriptionInvoice
        {
            SubscriptionId = subscription.Id,
            UserId = subscription.UserId,
            VendorId = subscription.VendorId,
            InvoiceNumber = invoiceNumber,
            GeneratedAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(subscription.BillingCycleDays),
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            TotalDeliveries = unbilledDeliveries.Count,
            Subtotal = subtotal,
            Tax = tax,
            Total = total,
            PaidAmount = 0,
            Status = InvoiceStatus.Generated,
            IsManuallyGenerated = true
        };

        _context.SubscriptionInvoices.Add(invoice);
        await _context.SaveChangesAsync(ct);

        // Link deliveries to invoice
        foreach (var delivery in unbilledDeliveries)
        {
            delivery.InvoiceId = invoice.Id;
        }
        await _context.SaveChangesAsync(ct);

        return MapToDto(invoice, subscription.SubscriptionNumber, unbilledDeliveries);
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken ct)
    {
        var today = DateTime.UtcNow;
        var prefix = $"INV-{today:yyyy-MMdd}";

        var lastInvoice = await _context.SubscriptionInvoices
            .Where(i => i.InvoiceNumber.StartsWith(prefix))
            .OrderByDescending(i => i.InvoiceNumber)
            .FirstOrDefaultAsync(ct);

        int sequence = 1;
        if (lastInvoice != null)
        {
            var lastSequence = lastInvoice.InvoiceNumber.Split('-').LastOrDefault();
            if (int.TryParse(lastSequence, out var parsed))
            {
                sequence = parsed + 1;
            }
        }

        return $"{prefix}-{sequence:D4}";
    }

    private static InvoiceDto MapToDto(SubscriptionInvoice invoice, string subscriptionNumber, List<Delivery> deliveries)
    {
        var lineItems = deliveries
            .SelectMany(d => d.Items.Select(i => new InvoiceLineItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                d.ScheduledDate,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            )))
            .ToList();

        return new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            SubscriptionId = invoice.SubscriptionId,
            SubscriptionNumber = subscriptionNumber,
            GeneratedAt = invoice.GeneratedAt,
            InvoiceDate = invoice.GeneratedAt,
            DueDate = invoice.DueDate,
            PeriodStart = invoice.PeriodStart,
            PeriodEnd = invoice.PeriodEnd,
            Subtotal = invoice.Subtotal,
            Tax = invoice.Tax,
            Total = invoice.Total,
            PaidAmount = invoice.PaidAmount,
            BalanceDue = invoice.BalanceDue,
            Status = invoice.Status.ToString(),
            InvoiceStatus = invoice.Status,
            PaidAt = invoice.PaidAt,
            PaymentMethodEnum = invoice.PaymentMethod,
            PaymentMethod = invoice.PaymentMethod?.ToString(),
            PaymentTransactionId = invoice.PaymentTransactionId,
            TotalDeliveries = deliveries.Count,
            LineItems = lineItems
        };
    }
}
