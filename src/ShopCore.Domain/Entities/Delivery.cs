namespace ShopCore.Domain.Entities;

public class Delivery : AuditableEntity
{
    public int SubscriptionId { get; set; }
    public int? InvoiceId { get; set; }

    public string DeliveryNumber { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }

    public DeliveryStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    // Total for entire delivery (all items)
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentTransactionId { get; set; }

    public string? DeliveryPersonName { get; set; }
    public string? FailureReason { get; set; }

    // Navigation
    public Subscription Subscription { get; set; } = null!;
    public SubscriptionInvoice? Invoice { get; set; }
    public ICollection<DeliveryItem> Items { get; set; } = new List<DeliveryItem>(); // ← NEW
}
