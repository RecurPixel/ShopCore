using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetAllVendors;

public class GetAllVendorsQueryHandler : IRequestHandler<GetAllVendorsQuery, PaginatedList<VendorProfileDto>>
{
    public Task<PaginatedList<VendorProfileDto>> Handle(GetAllVendorsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Fetch all approved vendors from database
        // 2. Filter by status if provided (active, suspended)
        // 3. Filter by category/services if provided
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Sort by rating or name
        // 6. Include vendor logo and basic stats
        // 7. Map to VendorProfileDto list
        // 8. Return PaginatedList<VendorProfileDto>
        return Task.FromResult(new PaginatedList<VendorProfileDto>([], 0, request.Page, request.PageSize));
    }
}
