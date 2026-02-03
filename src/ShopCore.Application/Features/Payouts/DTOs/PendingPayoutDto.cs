namespace ShopCore.Application.Payouts.DTOs;

public record PendingPayoutDto
{
    public decimal Amount { get; init; }
    public int OrderCount { get; init; }
    public int DeliveryCount { get; init; }
    public DateTime? EarliestOrderDate { get; init; }
    public DateTime? NextPayoutDate { get; init; }
}