using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.AddOneTimeItemToSubscription;

public record AddOneTimeItemToSubscriptionCommand(
    int SubscriptionId,
    int ProductId,
    int Quantity,
    DateTime DeliveryDate,
    PaymentOption Payment
) : IRequest<SubscriptionItemResultDto>;
