namespace ShopCore.Application.Coupons.DTOs;

public record CouponDto(
    int Id,
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
    int UsageCount,
    int? UsageLimitPerUser,
    bool IsActive,
    bool IsValid
);
