namespace ShopCore.Application.Payouts.DTOs;

public record VendorPayoutDto
{
    public int Id { get; init; }
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public DateTime PeriodFrom { get; init; }
    public DateTime PeriodTo { get; init; }
    public decimal GrossAmount { get; init; }
    public decimal PlatformFee { get; init; }
    public decimal NetAmount { get; init; }
    public PayoutStatus Status { get; init; }
    public string? TransactionId { get; init; }
    public DateTime? ProcessedAt { get; init; }
    public string? Notes { get; init; }
}