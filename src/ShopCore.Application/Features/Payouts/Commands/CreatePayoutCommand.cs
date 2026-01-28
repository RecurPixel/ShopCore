using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CreatePayout;

public record CreatePayoutCommand(
    int VendorId,
    decimal Amount,
    string? Notes
) : IRequest<PayoutDto>;
