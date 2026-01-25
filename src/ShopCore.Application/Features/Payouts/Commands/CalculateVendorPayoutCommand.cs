using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

public record CalculateVendorPayoutCommand(int VendorId, DateTime FromDate, DateTime ToDate)
    : IRequest<VendorPayoutDto>;
