namespace ShopCore.Application.Deliveries.Queries.GetSubscriptionDeliveries;

public class GetSubscriptionDeliveriesQueryHandler
    : IRequestHandler<GetSubscriptionDeliveriesQuery, object>
{
    public Task<object> Handle(
        GetSubscriptionDeliveriesQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
