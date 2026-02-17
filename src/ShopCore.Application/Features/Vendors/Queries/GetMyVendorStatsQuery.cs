using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorStats;

public record GetMyVendorStatsQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<VendorStatsDto>;
