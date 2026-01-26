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
        return Task.FromResult(new SubscriptionDto());
    }
}
