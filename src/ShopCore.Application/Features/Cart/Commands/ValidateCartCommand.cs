using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ValidateCart;

public record ValidateCartCommand : IRequest<CartValidationResultDto>;
