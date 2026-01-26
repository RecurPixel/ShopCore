using ShopCore.Application.Deliveries.DTOs;

namespace ShopCore.Application.Deliveries.Queries.GetSubscriptionDeliveries;

public class GetSubscriptionDeliveriesQueryHandler
    : IRequestHandler<GetSubscriptionDeliveriesQuery, List<DeliveryDto>>
{
    public Task<List<DeliveryDto>> Handle(
        GetSubscriptionDeliveriesQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
