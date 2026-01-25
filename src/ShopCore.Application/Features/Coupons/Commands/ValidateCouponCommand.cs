using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.ValidateCoupon;

public record ValidateCouponCommand(string Code, decimal CartTotal)
    : IRequest<CouponValidationResultDto>;
