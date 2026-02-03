namespace ShopCore.Application.Subscriptions.DTOs;

public record VendorSubscriptionDto
{
    public int Id { get; init; }
    public string SubscriptionNumber { get; init; } = string.Empty;
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string? CustomerPhone { get; init; }
    public string? DeliveryAddress { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Frequency { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime? NextDeliveryDate { get; init; }
    public List<SubscriptionItemDto> Items { get; init; } = new();
    public decimal TotalAmount { get; init; }
    public decimal UnpaidAmount { get; init; }
    public int ItemCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastDeliveryDate { get; init; }
}
