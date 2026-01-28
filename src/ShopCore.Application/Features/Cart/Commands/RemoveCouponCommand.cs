using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCoupon;

public record RemoveCouponCommand : IRequest<CartDto>;
