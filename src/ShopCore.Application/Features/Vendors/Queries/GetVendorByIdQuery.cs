using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorById;

public record GetVendorByIdQuery(int Id) : IRequest<VendorPublicProfileDto?>;
