namespace ShopCore.Application.Subscriptions.DTOs;

public record VendorSubscriptionDto(
    int Id,
    int CustomerId,
    string CustomerName,
    string? CustomerPhone,
    string DeliveryAddress,
    string Status,
    string Frequency,
    DateTime StartDate,
    DateTime? NextDeliveryDate,
    List<SubscriptionItemDto> Items,
    decimal TotalAmount,
    DateTime CreatedAt,
    DateTime? LastDeliveryDate
);
