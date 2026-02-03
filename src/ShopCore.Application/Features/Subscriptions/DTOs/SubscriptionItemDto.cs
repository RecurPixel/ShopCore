namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImage { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
    public decimal TotalPrice { get; init; }
    public bool IsRecurring { get; init; }
    public DateTime? OneTimeDeliveryDate { get; init; }
}
