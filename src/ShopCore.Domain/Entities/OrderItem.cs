namespace ShopCore.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int VendorId { get; set; }

    // Product snapshot (for historical accuracy - product might be deleted/changed)
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }
    public string? ProductImageUrl { get; set; }

    // Pricing
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; } // Price snapshot at order time

    // Commission (snapshot at order time)
    public decimal CommissionRate { get; set; } // 5.00 = 5%

    // Vendor Status (each vendor manages their items independently)
    public OrderItemStatus Status { get; set; } = OrderItemStatus.Pending;

    // Tracking fields
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? CancellationReason { get; set; }

    // Computed properties
    public decimal Subtotal => Quantity * UnitPrice;
    public decimal CommissionAmount => Subtotal * (CommissionRate / 100);
    public decimal VendorAmount => Subtotal - CommissionAmount;

    // Navigation
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public VendorProfile Vendor { get; set; } = null!;
}
