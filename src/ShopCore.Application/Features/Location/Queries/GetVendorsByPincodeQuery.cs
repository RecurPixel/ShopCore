using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Queries.GetVendorsByPincode;

public record GetVendorsByPincodeQuery(string Pincode) : IRequest<List<NearbyVendorDto>>;
