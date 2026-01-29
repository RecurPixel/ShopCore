using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler
    : IRequestHandler<CreateSubscriptionCommand, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        CreateSubscriptionCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Validate products/items exist and are available
        // 3. Calculate subscription details (price, frequency, etc.)
        // 4. Create Subscription entity with items
        // 5. Create initial delivery schedule
        // 6. Save to database
        // 7. Map and return SubscriptionDto
        return Task.FromResult(new SubscriptionDto());
    }
}
