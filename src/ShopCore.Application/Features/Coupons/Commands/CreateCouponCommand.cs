using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.CreateCoupon;

// Note: IsActive defaults to false - use ActivateCouponCommand to activate
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
    int? UsageLimitPerUser
) : IRequest<CouponDto>;
