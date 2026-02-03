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
        return new SubscriptionDto
        {
            Id = subscription.Id,
            SubscriptionNumber = subscription.SubscriptionNumber,
            UserId = subscription.UserId,
            VendorId = subscription.VendorId,
            VendorName = subscription.Vendor.BusinessName,
            DeliveryAddressId = subscription.DeliveryAddressId,
            DeliveryAddress = new AddressDto
            {
                Id = subscription.DeliveryAddress.Id,
                FullName = subscription.DeliveryAddress.FullName,
                PhoneNumber = subscription.DeliveryAddress.PhoneNumber,
                AddressLine1 = subscription.DeliveryAddress.AddressLine1,
                AddressLine2 = subscription.DeliveryAddress.AddressLine2,
                City = subscription.DeliveryAddress.City,
                State = subscription.DeliveryAddress.State,
                Pincode = subscription.DeliveryAddress.Pincode,
                Latitude = subscription.DeliveryAddress.Latitude,
                Longitude = subscription.DeliveryAddress.Longitude,
                IsDefault = subscription.DeliveryAddress.IsDefault
            },
            Frequency = subscription.Frequency.ToString(),
            CustomFrequencyDays = subscription.CustomFrequencyDays,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            NextDeliveryDate = subscription.NextDeliveryDate,
            PreferredDeliveryTime = subscription.PreferredDeliveryTime,
            BillingCycleDays = subscription.BillingCycleDays,
            NextBillingDate = subscription.NextBillingDate,
            UnpaidAmount = subscription.UnpaidAmount,
            CreditLimit = subscription.CreditLimit,
            DepositAmount = subscription.DepositAmount,
            DepositPaid = subscription.DepositPaid,
            DepositBalance = subscription.DepositBalance,
            Status = subscription.Status.ToString(),
            TotalDeliveries = subscription.TotalDeliveries,
            CompletedDeliveries = subscription.CompletedDeliveries,
            FailedDeliveries = subscription.FailedDeliveries,
            Items = subscription.Items.Select(i => new SubscriptionItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImageUrl = i.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }
}
