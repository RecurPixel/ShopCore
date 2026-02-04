using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptionById;

public class GetVendorSubscriptionByIdQueryHandler : IRequestHandler<GetVendorSubscriptionByIdQuery, VendorSubscriptionDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetVendorSubscriptionByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<VendorSubscriptionDto?> Handle(
        GetVendorSubscriptionByIdQuery request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.DeliveryAddress)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.Deliveries.OrderByDescending(d => d.ScheduledDate).Take(1))
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (subscription == null)
            return null;

        // Verify vendor owns this subscription
        if (subscription.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only view your own subscriptions");

        var address = subscription.DeliveryAddress;
        var fullAddress = $"{address.AddressLine1}, {address.City}, {address.State} - {address.Pincode}";

        var lastDelivery = subscription.Deliveries
            .Where(d => d.Status == DeliveryStatus.Delivered)
            .OrderByDescending(d => d.ActualDeliveryDate)
            .FirstOrDefault();

        var totalAmount = subscription.Items.Sum(i => i.Quantity * i.UnitPrice);

        return new VendorSubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.UserId,
            CustomerName = subscription.User.FullName,
            CustomerPhone = subscription.User.PhoneNumber,
            DeliveryAddress = fullAddress,
            Status = subscription.Status.ToString(),
            Frequency = subscription.Frequency.ToString(),
            StartDate = subscription.StartDate,
            NextDeliveryDate = subscription.NextDeliveryDate,
            Items = subscription.Items.Select(i => new SubscriptionItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImageUrl = i.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList(),
            TotalAmount = totalAmount,
            CreatedAt = subscription.CreatedAt,
            LastDeliveryDate = lastDelivery?.ActualDeliveryDate
        };
    }
}
