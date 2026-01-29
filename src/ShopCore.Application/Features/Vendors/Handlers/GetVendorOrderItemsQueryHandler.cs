using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorOrderItems;

public class GetVendorOrderItemsQueryHandler : IRequestHandler<GetVendorOrderItemsQuery, List<VendorOrderItemDto>>
{
    public Task<List<VendorOrderItemDto>> Handle(
        GetVendorOrderItemsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Find order by id
        // 3. Get only order items from this vendor
        // 4. Include product info, quantity, price
        // 5. Include item status (pending, shipped, delivered, etc.)
        // 6. Map to VendorOrderItemDto list and return
        return Task.FromResult(new List<VendorOrderItemDto>());
    }
}
