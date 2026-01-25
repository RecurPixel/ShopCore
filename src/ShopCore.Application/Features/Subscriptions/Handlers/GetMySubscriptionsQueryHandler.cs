namespace ShopCore.Application.Subscriptions.Queries.GetMySubscriptions;

public class GetMySubscriptionsQueryHandler : IRequestHandler<GetMySubscriptionsQuery, object>
{
    public Task<object> Handle(GetMySubscriptionsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
