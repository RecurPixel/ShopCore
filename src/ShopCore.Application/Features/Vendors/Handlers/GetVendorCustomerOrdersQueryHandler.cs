using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerOrders;

public class GetVendorCustomerOrdersQueryHandler : IRequestHandler<GetVendorCustomerOrdersQuery, PaginatedList<VendorOrderDto>>
{
    public Task<PaginatedList<VendorOrderDto>> Handle(
        GetVendorCustomerOrdersQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Find customer by user id
        // 3. Get orders containing items from this vendor
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Include only vendor's order items
        // 6. Sort by creation date (newest first)
        // 7. Map to VendorOrderDto list
        // 8. Return PaginatedList<VendorOrderDto>
        return Task.FromResult(new PaginatedList<VendorOrderDto>([], 0, request.Page, request.PageSize));
    }
}
