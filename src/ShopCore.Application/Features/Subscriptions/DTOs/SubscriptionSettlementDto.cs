namespace ShopCore.Application.Subscriptions.DTOs;

public record SubscriptionSettlementDto(
    int SubscriptionId,
    decimal TotalUnpaidAmount,
    decimal AmountSettled,
    decimal RemainingBalance,
    bool IsFullySettled,
    DateTime SettledAt
);
