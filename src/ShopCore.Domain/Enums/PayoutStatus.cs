namespace ShopCore.Domain.Enums;

public enum PayoutStatus
{
    Pending = 1, // Calculated but not processed
    Approved = 2, // Approved for payment
    Processing = 3, // Payment in progress
    Paid = 4, // Successfully paid
    Failed = 5, // Payment failed
    Cancelled = 6, // Payout cancelled
}
