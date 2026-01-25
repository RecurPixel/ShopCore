using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.ProcessVendorPayout;

public record ProcessVendorPayoutCommand(int PayoutId, string TransactionId, string? Notes)
    : IRequest<VendorPayoutDto>;
