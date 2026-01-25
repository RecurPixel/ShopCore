namespace ShopCore.Application.Subscriptions.Queries.GetSubscriptionById;

public class GetSubscriptionByIdQueryHandler : IRequestHandler<GetSubscriptionByIdQuery, object>
{
    public Task<object> Handle(
        GetSubscriptionByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
