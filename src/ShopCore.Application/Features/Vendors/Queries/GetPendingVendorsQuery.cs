using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetPendingVendors;

public record GetPendingVendorsQuery : IRequest<List<VendorProfileDto>>;
