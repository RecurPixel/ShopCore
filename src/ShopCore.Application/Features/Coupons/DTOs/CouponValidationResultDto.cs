namespace ShopCore.Application.Coupons.DTOs;

public record CouponValidationResultDto
{
    public bool IsValid { get; init; }
    public string? ErrorMessage { get; init; }
    public decimal DiscountAmount { get; init; }
    public CouponDto? Coupon { get; init; }
}
