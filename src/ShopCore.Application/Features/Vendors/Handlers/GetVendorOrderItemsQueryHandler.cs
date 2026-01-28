using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorOrderItems;

public class GetVendorOrderItemsQueryHandler : IRequestHandler<GetVendorOrderItemsQuery, List<VendorOrderItemDto>>
{
    public Task<List<VendorOrderItemDto>> Handle(
        GetVendorOrderItemsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
