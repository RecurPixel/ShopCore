using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetVendorCustomerById;

public record GetVendorCustomerByIdQuery(int UserId) : IRequest<VendorCustomerDetailDto?>;
