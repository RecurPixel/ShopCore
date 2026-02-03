namespace ShopCore.Application.Subscriptions.DTOs;

public record OneTimeDeliveryDto
{
    public int SubscriptionId { get; init; }
    public int DeliveryId { get; init; }
    public DateTime DeliveryDate { get; init; }
    public decimal TotalAmount { get; init; }
    public PaymentStatus PaymentStatus { get; init; }
}

public record SubscriptionItemResultDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public bool IsRecurring { get; init; }
    public DateTime? OneTimeDeliveryDate { get; init; }
    public bool IsDelivered { get; init; }
}
public enum PaymentOption
{
    AddToBill = 1,
    PayNow = 2,
    PayOnDelivery = 3
}

public record OrderItemInput
{
    public int ProductId { get; init; }
    public int Quantity { get; init; }
}