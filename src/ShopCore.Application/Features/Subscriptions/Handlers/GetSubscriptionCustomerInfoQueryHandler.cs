using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionCustomerInfo;

public class GetSubscriptionCustomerInfoQueryHandler : IRequestHandler<GetSubscriptionCustomerInfoQuery, SubscriptionCustomerInfoDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSubscriptionCustomerInfoQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubscriptionCustomerInfoDto?> Handle(
        GetSubscriptionCustomerInfoQuery request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .AsNoTracking()
            .Include(s => s.User)
            .Include(s => s.DeliveryAddress)
            .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, ct);

        if (subscription == null)
            return null;

        // Verify vendor owns this subscription
        if (subscription.VendorId != _currentUser.VendorId)
            throw new ForbiddenException("You can only view customer info for your own subscriptions");

        var address = subscription.DeliveryAddress;
        var fullAddress = $"{address.AddressLine1}, {address.AddressLine2}, {address.City}, {address.State} - {address.Pincode}";

        return new SubscriptionCustomerInfoDto
        {
            UserId = subscription.UserId,
            FullName = subscription.User.FullName,
            Email = subscription.User.Email,
            PhoneNumber = subscription.User.PhoneNumber,
            DeliveryAddress = fullAddress,
            PreferredDeliveryTime = subscription.PreferredDeliveryTime,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }
}
