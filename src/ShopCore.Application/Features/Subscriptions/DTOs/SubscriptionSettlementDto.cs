namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionSettlementDto
{
    public int SubscriptionId { get; init; }
    public decimal TotalDelivered { get; init; }
    public decimal TotalPaid { get; init; }
    public decimal DepositUsed { get; init; }
    public decimal NetBalance { get; init; }
    public string SettlementType { get; init; } = string.Empty; // "CustomerOwes", "VendorOwes", "Settled"
    public bool PaymentRequired { get; init; }
    public string? PaymentIntentId { get; init; }
    public decimal? PaymentAmount { get; init; }
    public decimal? RefundAmount { get; init; }
    public DateTime? SettledAt { get; init; }
}