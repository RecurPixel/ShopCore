using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.ConvertToSubscription;

public record ConvertToSubscriptionCommand(
    int OneTimeSubscriptionId,
    SubscriptionFrequency Frequency,
    int BillingCycleDays
) : IRequest<SubscriptionDto>;
