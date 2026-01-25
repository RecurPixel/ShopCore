using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.SettleSubscription;

public record SettleSubscriptionCommand(int Id) : IRequest<SubscriptionSettlementDto>;
