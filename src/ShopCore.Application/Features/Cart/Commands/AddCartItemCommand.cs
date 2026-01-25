using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.AddCartItem;

public record AddCartItemCommand(int ProductId, int Quantity) : IRequest<CartDto>;
