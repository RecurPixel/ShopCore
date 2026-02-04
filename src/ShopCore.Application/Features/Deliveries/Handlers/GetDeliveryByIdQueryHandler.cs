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
        var delivery = await _context.Deliveries
            .AsNoTracking()
            .Include(d => d.Subscription)
                .ThenInclude(s => s.User)
            .Include(d => d.Subscription)
                .ThenInclude(s => s.Vendor)
            .Include(d => d.Subscription)
                .ThenInclude(s => s.DeliveryAddress)
            .Include(d => d.Items)
                .ThenInclude(di => di.Product)
                    .ThenInclude(p => p.Images)
            .Include(d => d.Invoice)
            .Where(d => d.Id == request.Id
                     && (d.Subscription.UserId == _currentUser.UserId
                         || d.Subscription.VendorId == _currentUser.VendorId))
            .Select(d => new DeliveryDto
            {
                Id = d.Id,
                DeliveryNumber = d.DeliveryNumber,
                SubscriptionId = d.SubscriptionId,
                SubscriptionNumber = d.Subscription.SubscriptionNumber,

                // Customer Information
                CustomerName = (d.Subscription.User.FirstName + " " + d.Subscription.User.LastName).Trim(),
                CustomerPhone = d.Subscription.DeliveryAddress.PhoneNumber,

                // Delivery Address
                DeliveryAddress = d.Subscription.DeliveryAddress.AddressLine1,
                DeliveryCity = d.Subscription.DeliveryAddress.City,
                DeliveryState = d.Subscription.DeliveryAddress.State,
                Pincode = d.Subscription.DeliveryAddress.Pincode,
                Landmark = d.Subscription.DeliveryAddress.Landmark,

                // Delivery Details
                ScheduledDate = d.ScheduledDate,
                ActualDeliveryDate = d.ActualDeliveryDate,
                Status = d.Status.ToString(),
                DeliveryPersonName = d.DeliveryPersonName,
                DeliveryNotes = d.DeliveryNotes,
                FailureReason = d.FailureReason,

                // Proof of Delivery
                DeliveryPhotoUrl = d.DeliveryPhotoUrl,
                CustomerSignatureUrl = d.CustomerSignatureUrl,

                // Payment Information
                PaymentStatus = d.PaymentStatus.ToString(),
                Total = d.TotalAmount,
                PaymentMethod = d.PaymentMethod.HasValue ? d.PaymentMethod.Value.ToString() : null,
                PaymentGateway = d.PaymentGateway.ToString(),
                PaymentTransactionId = d.PaymentTransactionId,
                PaidAt = d.PaidAt,

                // Invoice
                InvoiceId = d.InvoiceId,
                InvoiceNumber = d.Invoice != null ? d.Invoice.InvoiceNumber : null,

                // Items
                ItemCount = d.Items.Count,
                Items = d.Items.Select(di => new DeliveryItemDto
                {
                    Id = di.Id,
                    ProductId = di.ProductId,
                    ProductName = di.ProductName,
                    ProductImageUrl = di.Product.Images.FirstOrDefault(i => i.IsPrimary)!.ImageUrl,
                    Quantity = di.Quantity,
                    UnitPrice = di.UnitPrice,
                    Subtotal = di.Amount,
                    Status = di.Status.ToString(),
                    Notes = di.Notes
                }).ToList(),

                // Metadata
                CreatedAt = d.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (delivery == null)
            throw new NotFoundException("Delivery", request.Id);

        return delivery;
    }
}