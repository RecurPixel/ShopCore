using ShopCore.Application.Location.DTOs;

namespace ShopCore.Application.Location.Queries.GetNearbyVendors;

public class GetNearbyVendorsQueryHandler : IRequestHandler<GetNearbyVendorsQuery, List<NearbyVendorDto>>
{
    private readonly IApplicationDbContext _context;

    public GetNearbyVendorsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<NearbyVendorDto>> Handle(GetNearbyVendorsQuery request, CancellationToken ct)
    {
        // Get all active vendors with their service areas
        var vendors = await _context.VendorProfiles
            .AsNoTracking()
            .Where(v => v.Status == VendorStatus.Active)
            .Include(v => v.ServiceAreas.Where(sa => sa.IsActive))
            .ToListAsync(ct);

        var nearbyVendors = new List<NearbyVendorDto>();

        foreach (var vendor in vendors)
        {
            // For now, calculate distance based on a simple approximation
            // In production, use actual vendor coordinates or geospatial queries
            var distance = CalculateDistance(
                request.Latitude, request.Longitude,
                request.Latitude, request.Longitude); // Placeholder - would need vendor coordinates

            // Check if vendor serves this area (within radius)
            var hasServiceArea = vendor.ServiceAreas.Any();

            if (hasServiceArea || distance <= request.RadiusKm)
            {
                nearbyVendors.Add(new NearbyVendorDto(
                    vendor.Id,
                    vendor.BusinessName,
                    vendor.BusinessLogo,
                    vendor.AverageRating,
                    vendor.TotalReviews,
                    distance,
                    IsVendorOpen(vendor),
                    vendor.ServiceAreas.Select(sa => sa.AreaName).ToList()
                ));
            }
        }

        return nearbyVendors
            .OrderBy(v => v.Distance)
            .ToList();
    }

    private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula
        const double R = 6371; // Earth's radius in km

        var dLat = ToRad(lat2 - lat1);
        var dLon = ToRad(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRad(double deg) => deg * Math.PI / 180;

    private static bool IsVendorOpen(VendorProfile vendor)
    {
        // Simple check - vendor is open if active
        // In production, would check business hours
        return vendor.Status == VendorStatus.Active;
    }
}
