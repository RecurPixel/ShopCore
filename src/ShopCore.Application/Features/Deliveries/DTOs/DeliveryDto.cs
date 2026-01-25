namespace ShopCore.Application.Deliveries.DTOs;

public record DeliveryDto(
    int Id,
    int SubscriptionId,
    string SubscriptionNumber,
    DateTime ScheduledDate,
    DateTime? DeliveredAt,
    DeliveryStatus Status,
    string? FailureReason,
    string? SkipReason,
    decimal TotalAmount,
    PaymentMethod? PaymentMethod,
    string? PaymentTransactionId,
    List<DeliveryItemDto> Items
);
