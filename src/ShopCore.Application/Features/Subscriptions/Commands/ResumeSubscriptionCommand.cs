using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

public record ResumeSubscriptionCommand(int Id) : IRequest<SubscriptionDto>;
