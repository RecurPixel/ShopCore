namespace ShopCore.Domain.Entities;

public class Coupon : AuditableEntity
{
    public string Code { get; set; } = string.Empty; // unique, "WELCOME10"
    public string? Description { get; set; } // "Get 10% off on first order"
    public CouponType Type { get; set; }

    // Discount values (based on type)
    public decimal? DiscountPercentage { get; set; } // For CouponType.Percentage
    public decimal? DiscountAmount { get; set; } // For CouponType.FixedAmount

    // Constraints
    public decimal? MinOrderValue { get; set; } // Minimum cart value required
    public decimal? MaxDiscount { get; set; } // Cap for percentage discounts

    // Validity
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }

    // Usage limits
    public int? UsageLimit { get; set; } // Total uses (null = unlimited)
    public int UsageCount { get; set; } // Times already used
    public int? UsageLimitPerUser { get; set; } // Per-user limit (null = unlimited)

    // Status
    public bool IsActive { get; set; } = true;

    // Computed property
    public bool IsValid =>
        IsActive
        && DateTime.UtcNow >= ValidFrom
        && DateTime.UtcNow <= ValidUntil
        && (!UsageLimit.HasValue || UsageCount < UsageLimit.Value);

    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
