namespace ShopCore.Application.Coupons.DTOs;

public record CouponDto
{
    public int Id { get; init; }
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public CouponType Type { get; init; }
    public decimal? DiscountPercentage { get; init; }
    public decimal? DiscountAmount { get; init; }
    public decimal? MinOrderValue { get; init; }
    public decimal? MaxDiscount { get; init; }
    public DateTime ValidFrom { get; init; }
    public DateTime ValidUntil { get; init; }
    public int? UsageLimit { get; init; }
    public int UsageCount { get; init; }
    public int? UsageLimitPerUser { get; init; }
    public bool IsActive { get; init; }
    public bool IsValid { get; init; }
}
