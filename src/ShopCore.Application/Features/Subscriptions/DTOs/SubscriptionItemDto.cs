namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }
    public bool IsRecurring { get; init; }
    public DateTime? OneTimeDeliveryDate { get; init; }
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
}
