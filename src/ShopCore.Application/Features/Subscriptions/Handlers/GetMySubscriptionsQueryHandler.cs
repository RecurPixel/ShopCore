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
        return Task.FromResult(new List<SubscriptionDto>());
    }
}
