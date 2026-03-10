using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetAllVendors;

public record GetAllVendorsQuery(
    string? Search = null,
    string? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<VendorProfileDto>>;
