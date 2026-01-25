namespace ShopCore.Domain.Entities;

public class VendorPayout : AuditableEntity
{
    public int VendorId { get; set; }
    public string PayoutNumber { get; set; } = string.Empty; // unique, "PAYOUT-2025-0124-001"

    // Period
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    // Financial breakdown
    public decimal TotalSales { get; set; } // Total order value for period
    public decimal CommissionAmount { get; set; } // Platform commission deducted
    public decimal DeductionsAmount { get; set; } // Other deductions (refunds, penalties, etc.)
    public decimal NetPayout { get; set; } // Final amount to vendor

    // Payment details
    public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; } // "Bank Transfer", "UPI", etc.
    public string? TransactionId { get; set; }
    public string? TransactionReference { get; set; }

    // Processing
    public int? ProcessedBy { get; set; } // FK to User (admin who processed)

    // Computed property
    public decimal CalculatedNetPayout => TotalSales - CommissionAmount - DeductionsAmount;

    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
    public User? ProcessedByUser { get; set; }
}
