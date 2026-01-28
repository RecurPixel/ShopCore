namespace ShopCore.Domain.Entities;

public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty; // unique, "ORD-2025-0124-001"
    public int UserId { get; set; }
    public int ShippingAddressId { get; set; }

    // Status (derived from OrderItems - use UpdateStatusFromItems())
    private OrderStatus _status = OrderStatus.Pending;
    public OrderStatus Status
    {
        get => _status;
        private set => _status = value;
    }

    // Amounts (stored for historical accuracy)
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; } // 18% GST
    public decimal Discount { get; set; }
    public decimal ShippingCharge { get; set; }
    public decimal Total { get; set; }

    // Refund tracking
    public decimal RefundedAmount { get; set; } = 0;

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

    /// <summary>
    /// Updates the Order status based on the statuses of all OrderItems.
    /// Call this method after updating any OrderItem status.
    /// </summary>
    public void UpdateStatusFromItems()
    {
        if (!Items.Any())
        {
            _status = OrderStatus.Pending;
            return;
        }

        var statuses = Items.Select(i => i.Status).ToList();

        // All cancelled
        if (statuses.All(s => s == OrderItemStatus.Cancelled))
        {
            _status = OrderStatus.Cancelled;
            return;
        }

        // Some cancelled
        if (statuses.Any(s => s == OrderItemStatus.Cancelled))
        {
            _status = OrderStatus.PartiallyCancelled;
            return;
        }

        // All delivered
        if (statuses.All(s => s == OrderItemStatus.Delivered))
        {
            _status = OrderStatus.Delivered;
            return;
        }

        // Some delivered
        if (statuses.Any(s => s == OrderItemStatus.Delivered))
        {
            _status = OrderStatus.PartiallyDelivered;
            return;
        }

        // All shipped
        if (statuses.All(s => s == OrderItemStatus.Shipped))
        {
            _status = OrderStatus.Shipped;
            return;
        }

        // Some shipped
        if (statuses.Any(s => s == OrderItemStatus.Shipped))
        {
            _status = OrderStatus.PartiallyShipped;
            return;
        }

        // At least one processing
        if (statuses.Any(s => s == OrderItemStatus.Processing))
        {
            _status = OrderStatus.Processing;
            return;
        }

        // All confirmed
        if (statuses.All(s => s == OrderItemStatus.Confirmed))
        {
            _status = OrderStatus.Confirmed;
            return;
        }

        _status = OrderStatus.Pending;
    }

    /// <summary>
    /// Sets the initial status (for EF Core and order creation)
    /// </summary>
    public void SetInitialStatus(OrderStatus status)
    {
        _status = status;
    }
}
