using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, List<ProductDto>>
{
    public Task<List<ProductDto>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<ProductDto>());
    }
}
