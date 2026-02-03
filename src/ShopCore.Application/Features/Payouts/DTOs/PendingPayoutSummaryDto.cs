namespace ShopCore.Application.Payouts.DTOs;

public record PendingPayoutSummaryDto
{
    public decimal TotalPayoutAmount { get; init; }
    public decimal TotalPendingAmount { get; init; }
    public int VendorCount { get; init; }
    public List<VendorPendingPayoutDto> VendorPayouts { get; init; } = new();
}

public record VendorPendingPayoutDto
{
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public decimal PendingAmount { get; init; }
    public int OrderCount { get; init; }
    public DateTime? OldestOrderDate { get; init; }
}
