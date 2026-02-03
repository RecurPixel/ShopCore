namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionSettlementDto
{
    public int SubscriptionId { get; init; }
    public decimal TotalUnpaidAmount { get; init; }
    public decimal AmountSettled { get; init; }
    public decimal RemainingBalance { get; init; }
    public bool IsFullySettled { get; init; }
    public DateTime? SettledAt { get; init; }
}