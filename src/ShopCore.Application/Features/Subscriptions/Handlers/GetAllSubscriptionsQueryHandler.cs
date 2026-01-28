using ShopCore.Application.Common.Models;
using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetAllSubscriptions;

public class GetAllSubscriptionsQueryHandler : IRequestHandler<GetAllSubscriptionsQuery, PaginatedList<SubscriptionDto>>
{
    public Task<PaginatedList<SubscriptionDto>> Handle(GetAllSubscriptionsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
