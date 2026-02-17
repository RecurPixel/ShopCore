using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.CreateSubscription;

public record CreateSubscriptionCommand(
    int VendorId,
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

