using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

// Note: VendorId is optional - if not provided (or 0), uses authenticated vendor's ID
// Admins can specify VendorId to calculate for any vendor
public record CalculateVendorPayoutCommand(DateTime FromDate, DateTime ToDate, int? VendorId = null)
    : IRequest<VendorPayoutDto>;
