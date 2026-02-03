using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.PauseSubscription;

public class PauseSubscriptionCommandHandler
    : IRequestHandler<PauseSubscriptionCommand, SubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public PauseSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubscriptionDto> Handle(
        PauseSubscriptionCommand request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .Include(s => s.Deliveries)
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.Id);

        if (subscription.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only pause your own subscriptions");

        if (subscription.Status != SubscriptionStatus.Active)
            throw new BadRequestException($"Cannot pause a subscription with status {subscription.Status}");

        subscription.Status = SubscriptionStatus.Paused;
        subscription.PausedAt = DateTime.UtcNow;

        // Skip upcoming scheduled deliveries
        var upcomingDeliveries = subscription.Deliveries
            .Where(d => d.Status == DeliveryStatus.Scheduled && d.ScheduledDate > DateTime.UtcNow);

        foreach (var delivery in upcomingDeliveries)
        {
            delivery.Status = DeliveryStatus.Skipped;
        }

        await _context.SaveChangesAsync(ct);

        return MapToDto(subscription);
    }

    private static SubscriptionDto MapToDto(Subscription subscription)
    {
        return new SubscriptionDto(
            subscription.Id,
            subscription.SubscriptionNumber,
            subscription.UserId,
            subscription.VendorId,
            subscription.Vendor.BusinessName,
            subscription.DeliveryAddressId,
            new AddressDto(
                subscription.DeliveryAddress.Id,
                subscription.DeliveryAddress.FullName,
                subscription.DeliveryAddress.PhoneNumber,
                subscription.DeliveryAddress.AddressLine1,
                subscription.DeliveryAddress.AddressLine2,
                subscription.DeliveryAddress.City,
                subscription.DeliveryAddress.State,
                subscription.DeliveryAddress.Pincode,
                subscription.DeliveryAddress.Latitude,
                subscription.DeliveryAddress.Longitude,
                subscription.DeliveryAddress.IsDefault
            ),
            subscription.Frequency,
            subscription.CustomFrequencyDays,
            subscription.StartDate,
            subscription.EndDate,
            subscription.NextDeliveryDate,
            subscription.PreferredDeliveryTime,
            subscription.BillingCycleDays,
            subscription.NextBillingDate,
            subscription.UnpaidAmount,
            subscription.CreditLimit,
            subscription.DepositAmount,
            subscription.DepositPaid,
            subscription.DepositBalance,
            subscription.Status,
            subscription.TotalDeliveries,
            subscription.CompletedDeliveries,
            subscription.FailedDeliveries,
            subscription.Items.Select(i => new SubscriptionItemDto(
                i.Id,
                i.ProductId,
                i.Product.Name,
                i.Product.ImageUrl,
                i.Quantity,
                i.UnitPrice,
                i.Quantity * i.UnitPrice
            )).ToList()
        );
    }
}
