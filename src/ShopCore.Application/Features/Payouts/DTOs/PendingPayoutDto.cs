namespace ShopCore.Application.Payouts.DTOs;

public record PendingPayoutDto(
    decimal Amount,
    int OrderCount,
    int DeliveryCount,
    DateTime? EarliestOrderDate,
    DateTime? NextPayoutDate
);
