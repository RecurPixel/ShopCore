using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.SearchProducts;

public record SearchProductsQuery(
    string? Q = null,
    int? CategoryId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<ProductDto>>;
