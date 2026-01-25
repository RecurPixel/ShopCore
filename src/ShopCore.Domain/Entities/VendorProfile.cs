namespace ShopCore.Domain.Entities;

public class VendorProfile : AuditableEntity
{
    // User Link
    public int UserId { get; set; } // FK, unique

    // Business Information
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string? BusinessLogo { get; set; }
    public string BusinessAddress { get; set; } = string.Empty;

    // Legal & Banking
    public string GstNumber { get; set; } = string.Empty; // unique
    public string PanNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankIfscCode { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;

    // Status & Commission
    public VendorStatus Status { get; set; } = VendorStatus.PendingApproval;
    public decimal CommissionRate { get; set; } = 5.00m; // percentage (5.00 = 5%)
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }

    // Subscription Settings (PRIVATE - for subscription feature)
    public bool RequiresDeposit { get; set; } = false;
    public decimal? DefaultDepositAmount { get; set; }
    public int? DefaultBillingCycleDays { get; set; }

    // Statistics (denormalized for performance)
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalProducts { get; set; } = 0;
    public int TotalOrders { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;

    // Navigation Properties
    public User User { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<VendorPayout> Payouts { get; set; } = new List<VendorPayout>();
}
