using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetVendorPayouts;

public record GetVendorPayoutsQuery : IRequest<List<VendorPayoutDto>>;
