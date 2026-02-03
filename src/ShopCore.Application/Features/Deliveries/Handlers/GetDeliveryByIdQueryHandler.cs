using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetDeliveryById;

public class GetDeliveryByIdQueryHandler : IRequestHandler<GetDeliveryByIdQuery, DeliveryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetDeliveryByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DeliveryDto> Handle(
        GetDeliveryByIdQuery request,
        CancellationToken cancellationToken)
    {
        // Note: Add DeliveryId to the query record
        var delivery = await _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Subscription)
                .ThenInclude(s => s.User)
            .Include(d => d.Subscription)
                .ThenInclude(s => s.Vendor)
            .Include(d => d.Items)
                .ThenInclude(di => di.Product)
                    .ThenInclude(p => p.Images)
            .Include(d => d.Invoice)
            .Where(d => d.Subscription.UserId == _currentUser.UserId
                     || d.Subscription.VendorId == _currentUser.VendorId)
            // Add: && d.Id == request.DeliveryId
            .Select(d => new DeliveryDto
            {
                Id = d.Id,
                DeliveryNumber = d.DeliveryNumber,
                SubscriptionId = d.SubscriptionId,
                ScheduledDate = d.ScheduledDate,
                ActualDeliveryDate = d.ActualDeliveryDate,
                Status = d.Status.ToString(),
                PaymentStatus = d.PaymentStatus.ToString(),
                TotalAmount = d.TotalAmount,
                PaymentMethod = d.PaymentMethod,
                PaidAt = d.PaidAt,
                PaymentTransactionId = d.PaymentTransactionId,
                DeliveryPersonName = d.DeliveryPersonName,
                FailureReason = d.FailureReason,
                InvoiceId = d.InvoiceId,
                InvoiceNumber = d.Invoice != null ? d.Invoice.InvoiceNumber : null,
                Items = d.Items.Select(di => new DeliveryItemDto
                {
                    Id = di.Id,
                    ProductId = di.ProductId,
                    ProductName = di.ProductName,
                    ProductImage = di.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                    Quantity = di.Quantity,
                    UnitPrice = di.UnitPrice,
                    Amount = di.Amount,
                    Status = di.Status.ToString(),
                    Notes = di.Notes
                }).ToList(),
                CreatedAt = d.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (delivery == null)
            throw new NotFoundException("Delivery not found");

        return delivery;
    }
}