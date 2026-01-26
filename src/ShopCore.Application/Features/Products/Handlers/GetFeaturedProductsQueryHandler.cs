using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetFeaturedProducts;

public class GetFeaturedProductsQueryHandler
    : IRequestHandler<GetFeaturedProductsQuery, List<ProductDto>>
{
    public Task<List<ProductDto>> Handle(
        GetFeaturedProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<ProductDto>());
    }
}
