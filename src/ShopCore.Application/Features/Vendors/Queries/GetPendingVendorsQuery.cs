using ShopCore.Application.Common.Models;
using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetPendingVendors;

public record GetPendingVendorsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<VendorProfileDto>>;
