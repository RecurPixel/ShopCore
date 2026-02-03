namespace ShopCore.Application.Payouts.DTOs;

public record PayoutDto
{
    public int Id { get; init; }
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Status { get; init; } = string.Empty;
    public string? TransactionReference { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ProcessedAt { get; init; }
}