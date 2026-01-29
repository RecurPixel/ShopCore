using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;

public class GetMySubscriptionsQueryHandler
    : IRequestHandler<GetMySubscriptionsQuery, List<SubscriptionDto>>
{
    public Task<List<SubscriptionDto>> Handle(
        GetMySubscriptionsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        // 1. Get current user from context
        // 2. Fetch user's subscriptions from database
        // 3. Filter by status if provided (active, paused, cancelled)
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Sort by creation date (newest first)
        // 6. Include subscription items
        // 7. Map to SubscriptionDto list and return
        return Task.FromResult(new List<SubscriptionDto>());
    }
}
