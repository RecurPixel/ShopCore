using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptionById;

public class GetVendorSubscriptionByIdQueryHandler : IRequestHandler<GetVendorSubscriptionByIdQuery, VendorSubscriptionDto?>
{
    public Task<VendorSubscriptionDto?> Handle(
        GetVendorSubscriptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Find subscription by id
        // 3. Verify subscription contains items from vendor's products
        // 4. Include customer info, items, delivery schedule
        // 5. Map to VendorSubscriptionDto and return
        return Task.FromResult((VendorSubscriptionDto?)null);
    }
}
