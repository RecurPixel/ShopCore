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
        // TODO: Implement query logic        // 1. Get current user from context
        // 2. Find subscription by id
        // 3. Verify user owns this subscription
        // 4. Fetch all deliveries for this subscription
        // 5. Filter by status if provided (scheduled, in-transit, delivered, failed)
        // 6. Filter by date range if provided
        // 7. Include delivery items and tracking info
        // 8. Sort by scheduled date and apply pagination
        // 9. Map to DeliveryDto list and return        return Task.FromResult<object>(new { });
    }
}
