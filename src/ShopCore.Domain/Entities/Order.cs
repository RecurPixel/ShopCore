namespace ShopCore.Domain.Entities;

public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty; // unique, "ORD-2025-0124-001"
    public int UserId { get; set; }
    public int ShippingAddressId { get; set; }

    // Status
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    // Amounts (stored for historical accuracy)
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; } // 18% GST
    public decimal Discount { get; set; }
    public decimal ShippingCharge { get; set; }
    public decimal Total { get; set; }

    // Payment
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public PaymentMethod PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime? PaidAt { get; set; }

    // Coupon
    public int? CouponId { get; set; }

    // Notes
    public string? CustomerNotes { get; set; } // Customer's delivery instructions
    public string? AdminNotes { get; set; } // Internal notes

    // Tracking
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }

    // Navigation
    public User User { get; set; } = null!;
    public Address ShippingAddress { get; set; } = null!;
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } =
        new List<OrderStatusHistory>();
}
