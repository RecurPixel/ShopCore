using ShopCore.Application.Subscriptions.DTOs;

namespace ShopCore.Application.Subscriptions.Queries.GetVendorSubscriptionById;

public class GetVendorSubscriptionByIdQueryHandler : IRequestHandler<GetVendorSubscriptionByIdQuery, VendorSubscriptionDto?>
{
    public Task<VendorSubscriptionDto?> Handle(
        GetVendorSubscriptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
