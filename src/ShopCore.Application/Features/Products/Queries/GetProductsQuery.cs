using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProducts;

public record GetProductsQuery : IRequest<List<ProductDto>>;
