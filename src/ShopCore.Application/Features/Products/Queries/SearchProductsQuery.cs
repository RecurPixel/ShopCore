using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.SearchProducts;

public record SearchProductsQuery : IRequest<List<ProductDto>>;
