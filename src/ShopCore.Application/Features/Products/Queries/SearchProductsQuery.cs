using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.SearchProducts;

public record SearchProductsQuery(
    string? Search = null,
    int? CategoryId = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<ProductDto>>;
