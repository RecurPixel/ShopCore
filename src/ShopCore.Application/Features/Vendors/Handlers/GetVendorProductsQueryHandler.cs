using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorProducts;

public class GetVendorProductsQueryHandler : IRequestHandler<GetVendorProductsQuery, PaginatedList<ProductDto>>
{
    public Task<PaginatedList<ProductDto>> Handle(
        GetVendorProductsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get vendor id from request (or current user if own products)
        // 2. Fetch vendor's products from database
        // 3. Filter by status (only public if request.publicOnly)
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Sort by creation date or rating
        // 6. Include product images and basic info
        // 7. Map to ProductDto list
        // 8. Return PaginatedList<ProductDto>
        return Task.FromResult(new PaginatedList<ProductDto>([], 0, request.Page, request.PageSize));
    }
}
