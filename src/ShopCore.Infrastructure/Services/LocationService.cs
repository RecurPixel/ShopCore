using Microsoft.EntityFrameworkCore;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Infrastructure.Data;

namespace ShopCore.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _context;

    public LocationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<GeocodeResult?> GeocodeAddressAsync(string address)
    {
        // TODO: Implement actual geocoding using Google Maps API, Mapbox, or similar
        // For now, return a mock response for development
        return Task.FromResult<GeocodeResult?>(new GeocodeResult
        {
            IsSuccess = true,
            FormattedAddress = address,
            Latitude = 12.9716, // Bangalore coordinates as default
            Longitude = 77.5946,
            City = "Bangalore",
            State = "Karnataka",
            Country = "India",
            Pincode = "560001"
        });
    }

    public Task<GeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude)
    {
        // TODO: Implement actual reverse geocoding
        return Task.FromResult<GeocodeResult?>(new GeocodeResult
        {
            IsSuccess = true,
            FormattedAddress = "Mock Address",
            Latitude = latitude,
            Longitude = longitude,
            City = "Bangalore",
            State = "Karnataka",
            Country = "India",
            Pincode = "560001"
        });
    }

    public async Task<bool> IsPointInServiceAreaAsync(double latitude, double longitude, int vendorId)
    {
        // Get vendor service areas by pincode matching
        // For full geofencing support, implement GeoJSON polygon checking
        var geocodeResult = await ReverseGeocodeAsync(latitude, longitude);
        if (geocodeResult == null || string.IsNullOrEmpty(geocodeResult.Pincode))
            return false;

        var serviceAreas = await _context.VendorServiceAreas
            .Where(sa => sa.VendorId == vendorId && sa.IsActive)
            .ToListAsync();

        foreach (var area in serviceAreas)
        {
            if (area.Pincodes.Contains(geocodeResult.Pincode))
            {
                return true;
            }
        }

        return false;
    }

    public Task<double> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2)
    {
        return Task.FromResult(CalculateHaversineDistance(lat1, lon1, lat2, lon2));
    }

    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in kilometers

        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c;
    }

    private static double ToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
