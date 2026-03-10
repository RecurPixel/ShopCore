using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetVendorInvoices;

public class GetVendorInvoicesQueryHandler : IRequestHandler<GetVendorInvoicesQuery, PaginatedList<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorInvoicesQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<PaginatedList<InvoiceDto>> Handle(
        GetVendorInvoicesQuery request,
        CancellationToken ct)
    {
        var query = _context.SubscriptionInvoices
            .AsNoTracking()
            .Where(i => i.VendorId == _currentUser.VendorId)
            .Include(i => i.Subscription)
            .Include(i => i.Deliveries)
                .ThenInclude(d => d.Items)
                    .ThenInclude(di => di.Product)
            .AsQueryable();

        // Filter by status
        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<InvoiceStatus>(request.Status, true, out var status))
        {
            query = query.Where(i => i.Status == status);
        }

        var totalCount = await query.CountAsync(ct);

        var invoices = await query
            .OrderByDescending(i => i.GeneratedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var items = invoices.Select(MapToDto).ToList();

        return new PaginatedList<InvoiceDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }

    private static InvoiceDto MapToDto(SubscriptionInvoice invoice)
    {
        PaymentMethod? paymentMethod = null;
        if (!string.IsNullOrEmpty(invoice.PaymentMethod.ToString()) &&
            Enum.TryParse<PaymentMethod>(invoice.PaymentMethod.ToString(), out var pm))
        {
            paymentMethod = pm;
        }

        var lineItems = invoice.Deliveries
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
            SubscriptionNumber = invoice.Subscription.SubscriptionNumber,
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
            PaymentMethodEnum = paymentMethod,
            PaymentMethod = invoice.PaymentMethod.ToString(),
            PaymentTransactionId = invoice.PaymentTransactionId,
            TotalDeliveries = invoice.Deliveries.Count,
            LineItems = lineItems
        };
    }
}
