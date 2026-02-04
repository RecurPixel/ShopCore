using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCartItem;

public record RemoveCartItemCommand(int CartItemId) : IRequest<CartDto>;
