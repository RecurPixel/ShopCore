namespace ShopCore.Application.Payouts.DTOs;

public record VendorPayoutDto(
    int Id,
    int VendorId,
    string VendorName,
    DateTime PeriodFrom,
    DateTime PeriodTo,
    decimal GrossAmount,
    decimal PlatformFee,
    decimal NetAmount,
    PayoutStatus Status,
    string? TransactionId,
    DateTime? ProcessedAt,
    string? Notes
);
