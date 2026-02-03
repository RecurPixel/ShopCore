namespace ShopCore.Application.Payments.DTOs;

public record PaymentHistoryDto
{
    public string PaymentId { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string ReferenceNumber { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public string? TransactionId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime PaidAt { get; init; }
    public string? Description { get; init; }
}
