using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;

public class GetSubscriptionByIdQueryHandler
    : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionDto>
{
    public Task<SubscriptionDto> Handle(
        GetSubscriptionByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        // 1. Get current user from context
        // 2. Fetch subscription by id from database
        // 3. Verify user has access (owner or vendor)
        // 4. Include subscription items, deliveries, invoices
        // 5. Map to SubscriptionDto and return
        return Task.FromResult(new SubscriptionDto());
    }
}
