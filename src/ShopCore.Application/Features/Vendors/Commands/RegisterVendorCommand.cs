using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.RegisterVendor;

public record RegisterVendorCommand(
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
    int? DefaultBillingCycleDays
) : IRequest<VendorProfileDto>;
