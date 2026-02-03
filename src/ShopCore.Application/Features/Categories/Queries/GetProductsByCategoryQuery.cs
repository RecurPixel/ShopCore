using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Categories.Queries.GetProductsByCategory;

public record GetProductsByCategoryQuery(
    int CategoryId = 0,
    int Page = 1,
    int PageSize = 20
) : IRequest<List<ProductDto>>;
