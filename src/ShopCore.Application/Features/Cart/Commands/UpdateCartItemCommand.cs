using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.UpdateCartItem;

public record UpdateCartItemCommand(int ProductId, int Quantity) : IRequest<CartDto>;
