namespace ShopCore.Domain.Entities;

public class Subscription : AuditableEntity
{
    public int UserId { get; set; }
    public int VendorId { get; set; } // Single vendor
    public int DeliveryAddressId { get; set; }

    public string SubscriptionNumber { get; set; } = string.Empty;

    // Subscription settings (SAME for all items)
    public SubscriptionFrequency Frequency { get; set; }
    public int? CustomFrequencyDays { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextDeliveryDate { get; set; }
    public string? PreferredDeliveryTime { get; set; }

    // Billing
    public int BillingCycleDays { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public decimal UnpaidAmount { get; set; }
    public decimal CreditLimit { get; set; } = 1200m;

    // Deposit (for entire subscription)
    public decimal? DepositAmount { get; set; }
    public decimal? DepositPaid { get; set; }
    public decimal? DepositBalance { get; set; }
    public DateTime? DepositPaidAt { get; set; }

    // One-Time Delivery Support
    public bool IsOneTimeDelivery { get; set; } = false;
    public bool AutoCancelAfterDelivery { get; set; } = false;

    // Status
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime? PausedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Statistics
    public int TotalDeliveries { get; set; }
    public int CompletedDeliveries { get; set; }
    public int FailedDeliveries { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public VendorProfile Vendor { get; set; } = null!;
    public Address DeliveryAddress { get; set; } = null!;
    public ICollection<SubscriptionItem> Items { get; set; } = new List<SubscriptionItem>(); // ← NEW
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public ICollection<SubscriptionInvoice> Invoices { get; set; } =
        new List<SubscriptionInvoice>();
}
