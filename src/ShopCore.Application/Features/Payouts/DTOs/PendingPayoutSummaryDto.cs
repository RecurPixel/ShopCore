namespace ShopCore.Application.Payouts.DTOs;

public record PendingPayoutSummaryDto(
    decimal TotalPendingAmount,
    int VendorCount,
    List<VendorPendingPayoutDto> VendorPayouts
);

public record VendorPendingPayoutDto(
    int VendorId,
    string VendorName,
    decimal PendingAmount,
    int OrderCount,
    DateTime? OldestOrderDate
);
