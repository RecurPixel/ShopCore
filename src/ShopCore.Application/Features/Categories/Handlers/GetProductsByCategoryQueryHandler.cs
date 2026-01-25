using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Categories.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler
    : IRequestHandler<GetProductsByCategoryQuery, List<ProductDto>>
{
    public Task<List<ProductDto>> Handle(
        GetProductsByCategoryQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<ProductDto>());
    }
}
