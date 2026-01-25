using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendor;

public record GetMyVendorQuery : IRequest<VendorProfileDto>;
