using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

public record ResumeSubscriptionCommand(int SubscriptionId) : IRequest<SubscriptionDto>;
