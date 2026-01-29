using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetAllSubscriptions;

public class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, PaginatedList<SubscriptionDto>>
{
    public Task<PaginatedList<SubscriptionDto>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Fetch all subscriptions from database
        // 2. Filter by status if provided (active, paused, cancelled)
        // 3. Apply pagination (request.Page, request.PageSize)
        // 4. Sort by creation date
        // 5. Include subscription items and customer info
        // 6. Map to SubscriptionDto list
        // 7. Return PaginatedList<SubscriptionDto>
        return Task.FromResult(new PaginatedList<SubscriptionDto>([], 0, request.Page, request.PageSize));
    }
}
