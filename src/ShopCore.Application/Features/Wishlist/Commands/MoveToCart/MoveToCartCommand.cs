using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Wishlist.Commands.MoveToCart;

public record MoveToCartCommand(int ProductId) : IRequest<CartDto>;
