namespace ShopCore.Application.Subscriptions.Commands.CancelSubscription;

public record CancelSubscriptionCommand(int SubscriptionId) : IRequest;
