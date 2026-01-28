namespace ShopCore.Domain.Enums;

public enum PaymentStatus
{
    Unpaid = 1,
    Pending = 2,
    Paid = 3,
    PartiallyRefunded = 4,
    Refunded = 5,
    Failed = 6
}
