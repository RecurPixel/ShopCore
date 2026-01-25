namespace ShopCore.Domain.Entities;

public class SubscriptionInvoice : AuditableEntity
{
    public int SubscriptionId { get; set; }
    public int UserId { get; set; }
    public int VendorId { get; set; }

    public string InvoiceNumber { get; set; } = string.Empty; // unique, "INV-2025-0124-001"
    public DateTime GeneratedAt { get; set; }
    public DateTime DueDate { get; set; }

    // Period covered
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalDeliveries { get; set; } // Quick reference count

    // Amounts
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; } // 18% GST
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; } // For partial payments

    // Status
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Generated;

    // Payment details
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }

    // Generation tracking
    public bool IsManuallyGenerated { get; set; } // true = vendor requested, false = auto-generated

    // Computed property
    public decimal BalanceDue => Total - PaidAmount;
    public bool IsFullyPaid => PaidAmount >= Total;
    public bool IsOverdue =>
        Status == InvoiceStatus.Overdue
        || (Status != InvoiceStatus.Paid && DateTime.UtcNow > DueDate);

    // Navigation
    public Subscription Subscription { get; set; } = null!;
    public User User { get; set; } = null!;
    public VendorProfile Vendor { get; set; } = null!;
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
