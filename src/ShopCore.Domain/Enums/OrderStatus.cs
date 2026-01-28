namespace ShopCore.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    PaymentFailed = 2,
    Confirmed = 3,
    Processing = 4,
    PartiallyShipped = 5,
    Shipped = 6,
    PartiallyDelivered = 7,
    Delivered = 8,
    Completed = 9,
    Cancelled = 10,
    PartiallyCancelled = 11,
    Refunded = 12,
    PartiallyRefunded = 13
}
