namespace ShopCore.Application.Subscriptions.DTOs;

public record OneTimeDeliveryDto(
    int SubscriptionId,
    int DeliveryId,
    DateTime DeliveryDate,
    decimal TotalAmount,
    PaymentStatus PaymentStatus
);

public record SubscriptionItemResultDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    bool IsRecurring,
    DateTime? OneTimeDeliveryDate,
    bool IsDelivered
);

public enum PaymentOption
{
    AddToBill = 1,
    PayNow = 2,
    PayOnDelivery = 3
}

public record OrderItemInput(
    int ProductId,
    int Quantity
);
