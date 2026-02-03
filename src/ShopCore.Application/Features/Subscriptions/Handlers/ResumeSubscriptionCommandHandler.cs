using ShopCore.Application.Addresses.DTOs;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

public class ResumeSubscriptionCommandHandler
    : IRequestHandler<ResumeSubscriptionCommand, SubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ResumeSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<SubscriptionDto> Handle(
        ResumeSubscriptionCommand request,
        CancellationToken ct)
    {
        var subscription = await _context.Subscriptions
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .Include(s => s.Items)
                .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.Id);

        if (subscription.UserId != _currentUser.UserId)
            throw new ForbiddenException("You can only resume your own subscriptions");

        if (subscription.Status != SubscriptionStatus.Paused)
            throw new BadRequestException($"Can only resume paused subscriptions. Current status: {subscription.Status}");

        subscription.Status = SubscriptionStatus.Active;
        subscription.PausedAt = null;

        // Calculate next delivery date based on frequency
        subscription.NextDeliveryDate = CalculateNextDeliveryDate(subscription);

        await _context.SaveChangesAsync(ct);

        return MapToDto(subscription);
    }

    private static DateTime CalculateNextDeliveryDate(Subscription subscription)
    {
        var today = DateTime.UtcNow.Date;
        return subscription.Frequency switch
        {
            SubscriptionFrequency.Daily => today.AddDays(1),
            SubscriptionFrequency.AlternateDays => today.AddDays(2),
            SubscriptionFrequency.Weekly => today.AddDays(7),
            SubscriptionFrequency.BiWeekly => today.AddDays(14),
            SubscriptionFrequency.Monthly => today.AddMonths(1),
            SubscriptionFrequency.Custom => today.AddDays(subscription.CustomFrequencyDays ?? 7),
            _ => today.AddDays(1)
        };
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
