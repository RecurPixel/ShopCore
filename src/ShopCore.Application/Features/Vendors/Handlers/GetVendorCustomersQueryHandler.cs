using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomers;

public class GetVendorCustomersQueryHandler : IRequestHandler<GetVendorCustomersQuery, PaginatedList<VendorCustomerDto>>
{
    public Task<PaginatedList<VendorCustomerDto>> Handle(
        GetVendorCustomersQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Fetch all customers who ordered from this vendor
        // 3. Filter by search term if provided
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Include customer contact info, total spent, order count
        // 6. Sort by recent order or total spent
        // 7. Map to VendorCustomerDto list
        // 8. Return PaginatedList<VendorCustomerDto>
        return Task.FromResult(new PaginatedList<VendorCustomerDto>([], 0, request.Page, request.PageSize));
    }
}
