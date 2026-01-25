using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery : IRequest<ProductDetailDto>;
