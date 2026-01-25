using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.CreateCoupon;

public record CreateCouponCommand(
    string Code,
    string? Description,
    CouponType Type,
    decimal? DiscountPercentage,
    decimal? DiscountAmount,
    decimal? MinOrderValue,
    decimal? MaxDiscount,
    DateTime ValidFrom,
    DateTime ValidUntil,
    int? UsageLimit,
    int? UsageLimitPerUser,
    bool IsActive
) : IRequest<CouponDto>;
