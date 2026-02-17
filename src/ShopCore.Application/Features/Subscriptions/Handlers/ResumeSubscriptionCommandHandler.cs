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
            .FirstOrDefaultAsync(s => s.Id == request.SubscriptionId, ct);

        if (subscription == null)
            throw new NotFoundException("Subscription", request.SubscriptionId);

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
            SubscriptionFrequency.EveryTwoDays => today.AddDays(2),
            SubscriptionFrequency.Weekly => today.AddDays(7),
            SubscriptionFrequency.BiWeekly => today.AddDays(14),
            SubscriptionFrequency.Monthly => today.AddMonths(1),
            SubscriptionFrequency.Custom => today.AddDays(subscription.CustomFrequencyDays ?? 7),
            _ => today.AddDays(1)
        };
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
                ProductImageUrl = i.Product.Images.FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }
}
