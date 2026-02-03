namespace ShopCore.Application.Orders.DTOs;

public record CancellationResultDto
{
    public int OrderId { get; init; }
    public int? OrderItemId { get; init; }
    public DateTime CancelledAt { get; init; }
    public decimal RefundAmount { get; init; }
    public string RefundStatus { get; init; } = string.Empty;
}

public record RefundResultDto
{
    public int OrderId { get; init; }
    public decimal RefundAmount { get; init; }
    public string? RefundTransactionId { get; init; }
    public DateTime RefundedAt { get; init; }
    public string Status { get; init; } = string.Empty;
}