using ShopCore.Application.Common.Models;
using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PaginatedList<ProductDto>>
{
    public Task<PaginatedList<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Fetch all products from database
        // 2. Filter by vendor if provided
        // 3. Filter by status (only published/approved)
        // 4. Apply pagination (request.Page, request.PageSize)
        // 5. Sort by creation date or rating
        // 6. Include product images and vendor info
        // 7. Map to ProductDto list
        // 8. Return PaginatedList<ProductDto>
        return Task.FromResult(new PaginatedList<ProductDto>([], 0, request.Page, request.PageSize));
    }
}
