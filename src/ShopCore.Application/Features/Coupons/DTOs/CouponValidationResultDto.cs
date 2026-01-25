namespace ShopCore.Application.Coupons.DTOs;

public record CouponValidationResultDto(
    bool IsValid,
    string? ErrorMessage,
    decimal DiscountAmount,
    CouponDto? Coupon
);
