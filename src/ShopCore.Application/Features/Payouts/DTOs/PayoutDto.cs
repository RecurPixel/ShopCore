namespace ShopCore.Application.Payouts.DTOs;

public record PayoutDto(
    int Id,
    int VendorId,
    string VendorName,
    decimal Amount,
    string Status,
    string? TransactionReference,
    string? Notes,
    DateTime CreatedAt,
    DateTime? ProcessedAt
);
