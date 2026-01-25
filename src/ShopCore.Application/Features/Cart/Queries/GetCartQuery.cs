using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Queries.GetCart;

public record GetCartQuery : IRequest<CartDto>;
