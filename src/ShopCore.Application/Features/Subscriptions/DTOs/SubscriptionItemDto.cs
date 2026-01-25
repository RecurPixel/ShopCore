namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);
