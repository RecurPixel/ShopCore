using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Queries.GetNearbyVendors;

public record GetNearbyVendorsQuery(
    double Latitude,
    double Longitude,
    double RadiusKm = 5
) : IRequest<List<NearbyVendorDto>>;
