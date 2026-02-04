using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.UpdateCartItem;

public record UpdateCartItemCommand(int Quantity) : IRequest<CartDto>
{
    public int CartItemId { get; init; }
}
