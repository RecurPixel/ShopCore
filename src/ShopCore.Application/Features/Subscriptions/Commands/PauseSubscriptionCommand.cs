using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.PauseSubscription;

public record PauseSubscriptionCommand(int Id) : IRequest<SubscriptionDto>;
