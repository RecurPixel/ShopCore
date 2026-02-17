using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.CreateSubscription;

// Note: VendorId is derived from the products - all products must be from the same vendor
public record CreateSubscriptionCommand(
    int DeliveryAddressId,
    List<SubscriptionItemDto> Items,
    SubscriptionFrequency Frequency,
    int? CustomFrequencyDays,
    DateTime StartDate,
    string? PreferredDeliveryTime,
    int BillingCycleDays,
    decimal? DepositAmount,
    decimal? CreditLimit
) : IRequest<SubscriptionDto>;

