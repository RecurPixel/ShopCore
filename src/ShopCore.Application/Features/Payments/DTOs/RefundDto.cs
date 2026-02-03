namespace ShopCore.Application.Payments.DTOs;

public record RefundDto
{
    public string RefundId { get; init; } = string.Empty;
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public decimal RefundAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime? RefundedAt { get; init; }
}
