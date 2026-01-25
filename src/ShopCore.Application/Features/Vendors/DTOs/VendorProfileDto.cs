namespace ShopCore.Application.Vendors.DTOs;

public record VendorProfileDto(
    int Id,
    int UserId,
    string BusinessName,
    string? BusinessDescription,
    string? BusinessLogo,
    string BusinessAddress,
    string GstNumber,
    string PanNumber,
    string BankName,
    string BankAccountNumber,
    string BankIfscCode,
    string BankAccountHolderName,
    bool RequiresDeposit,
    decimal? DefaultDepositAmount,
    int? DefaultBillingCycleDays,
    VendorStatus Status,
    decimal Rating,
    int TotalReviews,
    int TotalProducts,
    int TotalOrders,
    DateTime CreatedAt
);
