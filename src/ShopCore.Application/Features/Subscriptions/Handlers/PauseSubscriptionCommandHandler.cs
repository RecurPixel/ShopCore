using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.PauseSubscription;

public class PauseSubscriptionCommandHandler
    : IRequestHandler<PauseSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        PauseSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find subscription by id
        // 3. Verify user owns the subscription
        // 4. Check subscription status (can't pause if already paused/cancelled)
        // 5. Mark subscription as paused
        // 6. Cancel/skip next deliveries
        // 7. Save changes to database
        // 8. Map and return updated SubscriptionDto
        return Task.FromResult(new SubscriptionDto());
    }
}
