using ShopCore.Application.Invoices.DTOs;

namespace ShopCore.Application.Invoices.Queries.GetInvoiceById;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetInvoiceByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<InvoiceDto> Handle(
        GetInvoiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        var invoice = await _context.SubscriptionInvoices
            .AsNoTracking()
            .Include(i => i.Subscription)
                .ThenInclude(s => s.User)
            .Include(i => i.Subscription)
                .ThenInclude(s => s.Vendor)
            .Include(i => i.Subscription)
                .ThenInclude(s => s.DeliveryAddress)
            .Include(i => i.Deliveries)
                .ThenInclude(d => d.Items)
            .Where(i => i.Id == request.InvoiceId
                && (i.UserId == _currentUser.UserId || i.VendorId == _currentUser.VendorId))
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                SubscriptionId = i.SubscriptionId,
                SubscriptionNumber = i.Subscription.SubscriptionNumber,
                UserId = i.UserId,
                CustomerName = i.User.FirstName + " " + i.User.LastName,
                CustomerEmail = i.User.Email,
                CustomerPhone = i.User.PhoneNumber,
                VendorId = i.VendorId,
                VendorName = i.Vendor.BusinessName,
                VendorAddress = i.Vendor.BusinessAddress,
                VendorGst = i.Vendor.GstNumber,
                GeneratedAt = i.GeneratedAt,
                DueDate = i.DueDate,
                PaidAt = i.PaidAt,
                PeriodStart = i.PeriodStart,
                PeriodEnd = i.PeriodEnd,
                TotalDeliveries = i.TotalDeliveries,
                Subtotal = i.Subtotal,
                Tax = i.Tax,
                Total = i.Total,
                PaidAmount = i.PaidAmount,
                BalanceDue = i.BalanceDue,
                Status = i.Status.ToString(),
                PaymentMethod = i.PaymentMethod,
                PaymentTransactionId = i.PaymentTransactionId,
                IsManuallyGenerated = i.IsManuallyGenerated,
                IsOverdue = i.IsOverdue,
                IsFullyPaid = i.IsFullyPaid,
                Deliveries = i.Deliveries.Select(d => new InvoiceDeliveryDto
                {
                    DeliveryId = d.Id,
                    DeliveryNumber = d.DeliveryNumber,
                    ScheduledDate = d.ScheduledDate,
                    ActualDeliveryDate = d.ActualDeliveryDate,
                    TotalAmount = d.TotalAmount,
                    ItemCount = d.Items.Count
                }).ToList(),
                CreatedAt = i.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (invoice == null)
            throw new NotFoundException("Invoice not found");

        return invoice;
    }
}
