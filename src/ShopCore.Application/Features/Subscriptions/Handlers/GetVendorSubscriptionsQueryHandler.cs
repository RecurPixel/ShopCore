namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptions;

public class GetVendorSubscriptionsQueryHandler
    : IRequestHandler<GetVendorSubscriptionsQuery, object>
{
    public Task<object> Handle(
        GetVendorSubscriptionsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
