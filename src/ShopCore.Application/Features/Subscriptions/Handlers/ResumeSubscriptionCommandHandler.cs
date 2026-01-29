using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.ResumeSubscription;

public class ResumeSubscriptionCommandHandler
    : IRequestHandler<ResumeSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        ResumeSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find subscription by id
        // 3. Verify user owns the subscription
        // 4. Check subscription is paused (not cancelled or active)
        // 5. Mark subscription as active
        // 6. Resume delivery schedule
        // 7. Save changes to database
        // 8. Map and return updated SubscriptionDto
        return Task.FromResult(new SubscriptionDto());
    }
}
