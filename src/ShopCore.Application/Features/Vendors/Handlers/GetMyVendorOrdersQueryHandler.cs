using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorOrders;

public class GetMyVendorOrdersQueryHandler
    : IRequestHandler<GetMyVendorOrdersQuery, List<VendorOrderDto>>
{
    public Task<List<VendorOrderDto>> Handle(
        GetMyVendorOrdersQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<VendorOrderDto>());
    }
}
