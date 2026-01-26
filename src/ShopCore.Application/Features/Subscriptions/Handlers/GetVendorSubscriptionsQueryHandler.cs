using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;

public class GetVendorSubscriptionsQueryHandler
    : IRequestHandler<GetVendorSubscriptionsQuery, List<SubscriptionDto>>
{
    public Task<List<SubscriptionDto>> Handle(
        GetVendorSubscriptionsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<SubscriptionDto>());
    }
}
