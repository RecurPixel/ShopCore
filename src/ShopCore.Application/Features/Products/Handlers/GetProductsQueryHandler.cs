using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public Task<List<ProductDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        // 1. Fetch products from database
        // 2. Filter by category if provided
        // 3. Filter by vendor if provided
        // 4. Filter only active/approved products
        // 5. Apply pagination (request.Page, request.PageSize)
        // 6. Sort by relevance/creation date
        // 7. Include product images
        // 8. Map to ProductDto list and return
        return Task.FromResult(new List<ProductDto>());
    }
}
