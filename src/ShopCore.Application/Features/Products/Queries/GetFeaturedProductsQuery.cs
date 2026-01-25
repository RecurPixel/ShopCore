using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetFeaturedProducts;

public record GetFeaturedProductsQuery : IRequest<List<ProductDto>>;
