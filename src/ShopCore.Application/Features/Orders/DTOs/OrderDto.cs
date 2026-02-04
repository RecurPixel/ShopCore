namespace ShopCore.Application.Orders.DTOs;

public record OrderDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    public decimal ShippingCharge { get; init; }
    public decimal Total { get; init; }
    public int ItemCount { get; init; }
    public DateTime CreatedAt { get; init; }
}
