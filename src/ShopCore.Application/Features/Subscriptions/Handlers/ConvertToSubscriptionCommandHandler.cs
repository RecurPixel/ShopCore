using ShopCore.Application.Subscriptions.Commands.ConvertToSubscription;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Handlers;

public class ConvertToSubscriptionCommandHandler
    : IRequestHandler<ConvertToSubscriptionCommand, SubscriptionDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ConvertToSubscriptionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<SubscriptionDto> Handle(
        ConvertToSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var subscription = await _context.Subscriptions
            .Include(s => s.Items)
            .Include(s => s.Vendor)
            .Include(s => s.DeliveryAddress)
            .FirstOrDefaultAsync(s => s.Id == request.OneTimeSubscriptionId && s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(Subscription), request.OneTimeSubscriptionId);

        if (!subscription.IsOneTimeDelivery)
        {
            throw new InvalidOperationException("This is already a recurring subscription");
        }

        // Convert to recurring subscription
        subscription.IsOneTimeDelivery = false;
        subscription.AutoCancelAfterDelivery = false;
        subscription.Frequency = request.Frequency;
        subscription.BillingCycleDays = request.BillingCycleDays;
        subscription.NextBillingDate = DateTime.UtcNow.AddDays(request.BillingCycleDays);

        // Convert all items to recurring
        foreach (var item in subscription.Items)
        {
            item.IsRecurring = true;
            item.OneTimeDeliveryDate = null;
        }

        // Calculate next delivery based on frequency
        subscription.NextDeliveryDate = CalculateNextDeliveryDate(DateTime.UtcNow, request.Frequency);

        await _context.SaveChangesAsync(cancellationToken);

        // Return updated subscription
        return new SubscriptionDto
        {
            Id = subscription.Id,
            SubscriptionNumber = subscription.SubscriptionNumber,
            UserId = subscription.UserId,
            VendorId = subscription.VendorId,
            VendorName = subscription.Vendor.BusinessName,
            DeliveryAddressId = subscription.DeliveryAddressId,
            DeliveryAddress = null, // AddressDto - simplified for now
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
            Status = subscription.Status,
            TotalDeliveries = subscription.TotalDeliveries,
            CompletedDeliveries = subscription.CompletedDeliveries,
            FailedDeliveries = subscription.FailedDeliveries,
            Items = subscription.Items.Select(i => new SubscriptionItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                ProductImage = i.Product.Images.FirstOrDefault()?.ImageUrl ?? string.Empty,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }

    private static DateTime CalculateNextDeliveryDate(DateTime from, SubscriptionFrequency frequency)
    {
        return frequency switch
        {
            SubscriptionFrequency.Daily => from.AddDays(1),
            SubscriptionFrequency.EveryTwoDays => from.AddDays(2),
            SubscriptionFrequency.Weekly => from.AddDays(7),
            SubscriptionFrequency.BiWeekly => from.AddDays(14),
            SubscriptionFrequency.Monthly => from.AddMonths(1),
            _ => from.AddDays(1)
        };
    }
}
