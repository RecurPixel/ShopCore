namespace ShopCore.Application.Deliveries.DTOs;

public record DeliveryItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
}
