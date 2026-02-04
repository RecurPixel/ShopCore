namespace ShopCore.Application.Orders.DTOs;

public record OrderDetailDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public int UserId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string? CustomerEmail { get; init; }
    public string? CustomerPhone { get; init; }
    public OrderAddressDto ShippingAddress { get; init; } = null!;
    public string Status { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string PaymentGateway { get; init; } = string.Empty;
    public string? PaymentTransactionId { get; init; }
    public DateTime? PaidAt { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    public decimal ShippingCharge { get; init; }
    public decimal Total { get; init; }
    public string? CouponCode { get; init; }
    public string? CustomerNotes { get; init; }
    public string? AdminNotes { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public DateTime? CancelledAt { get; init; }
    public string? CancellationReason { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
    public List<OrderStatusHistoryDto> StatusHistory { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}
