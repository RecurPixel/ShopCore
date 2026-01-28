namespace ShopCore.Application.Orders.DTOs;

public record CancellationResultDto(
    int OrderId,
    int? OrderItemId,
    DateTime CancelledAt,
    decimal RefundAmount,
    string RefundStatus
);

public record RefundResultDto(
    int OrderId,
    decimal RefundAmount,
    string? RefundTransactionId,
    DateTime RefundedAt,
    string Status
);
