using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.SearchVendorsByLocation;

public record SearchVendorsByLocationQuery(
    string? Pincode = null,
    double? Latitude = null,
    double? Longitude = null,
    int? CategoryId = null,
    int RadiusKm = 5
) : IRequest<List<VendorSearchResultDto>>;
