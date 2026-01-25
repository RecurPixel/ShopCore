using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.UpdateMyVendor;

public record UpdateMyVendorCommand(
    string BusinessName,
    string? BusinessDescription,
    string? BusinessLogo,
    string BusinessAddress,
    string BankName,
    string BankAccountNumber,
    string BankIfscCode,
    string BankAccountHolderName,
    bool RequiresDeposit,
    decimal? DefaultDepositAmount,
    int? DefaultBillingCycleDays
) : IRequest<VendorProfileDto>;
