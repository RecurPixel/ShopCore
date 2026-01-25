using ShopCore.Application.Products.DTOs;

namespace ShopCore.Application.Products.Commands.UpdateProductStatus;

public record UpdateProductStatusCommand(int Id, ProductStatus Status) : IRequest<ProductDto>;
