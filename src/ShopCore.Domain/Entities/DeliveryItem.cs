namespace ShopCore.Domain.Entities;

public class DeliveryItem
{
    public int Id { get; set; }
    public int DeliveryId { get; set; }
    public int SubscriptionItemId { get; set; }
    public int ProductId { get; set; }

    // Snapshot
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }

    // Status (per-item in case of partial delivery)
    public DeliveryItemStatus Status { get; set; } = DeliveryItemStatus.Scheduled;
    public string? Notes { get; set; } // "Out of stock", "Delivered damaged"

    // Navigation
    public Delivery Delivery { get; set; } = null!;
    public SubscriptionItem SubscriptionItem { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
