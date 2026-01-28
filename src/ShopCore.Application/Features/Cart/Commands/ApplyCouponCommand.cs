using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ApplyCoupon;

public record ApplyCouponCommand(string CouponCode) : IRequest<CartDto>;
