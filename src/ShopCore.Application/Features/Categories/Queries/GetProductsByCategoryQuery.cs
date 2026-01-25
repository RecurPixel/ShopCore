using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Categories.Queries.GetProductsByCategory;

public record GetProductsByCategoryQuery : IRequest<List<ProductDto>>;
