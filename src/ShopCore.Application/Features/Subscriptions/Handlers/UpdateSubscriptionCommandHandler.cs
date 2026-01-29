using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler
    : IRequestHandler<UpdateSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        UpdateSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find subscription by id
        // 3. Verify user owns the subscription
        // 4. Update subscription items/frequency/address as needed
        // 5. Recalculate charges if items changed
        // 6. Save changes to database
        // 7. Map and return updated SubscriptionDto
        return Task.FromResult(new SubscriptionDto());
    }
}
