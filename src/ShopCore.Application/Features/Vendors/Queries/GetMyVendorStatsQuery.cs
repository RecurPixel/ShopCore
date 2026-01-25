using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorStats;

public record GetMyVendorStatsQuery : IRequest<VendorStatsDto>;
