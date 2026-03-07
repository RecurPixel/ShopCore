using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Infrastructure.Data;
using System.Text.Json;

namespace ShopCore.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly ApplicationDbContext _context;
    private readonly HttpClient _httpClient;
    private readonly GoogleMapsSettings _settings;

    public LocationService(
        ApplicationDbContext context,
        HttpClient httpClient,
        IOptions<GoogleMapsSettings> settings)
    {
        _context = context;
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<GeocodeResult?> GeocodeAddressAsync(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return new GeocodeResult
            {
                IsSuccess = false,
                ErrorMessage = "Address cannot be empty"
            };
        }

        // If no API key is configured, use fallback mock data for development
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            return GetMockGeocodeResult(address);
        }

        try
        {
            var encodedAddress = Uri.EscapeDataString(address);
            var url = $"{_settings.BaseUrl}/geocode/json?address={encodedAddress}&key={_settings.ApiKey}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var googleResponse = JsonSerializer.Deserialize<GoogleGeocodeResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (googleResponse?.Status != "OK" || googleResponse.Results == null || googleResponse.Results.Length == 0)
            {
                return new GeocodeResult
                {
                    IsSuccess = false,
                    ErrorMessage = googleResponse?.Status == "ZERO_RESULTS"
                        ? "No results found for the given address"
                        : $"Geocoding failed: {googleResponse?.Status}"
                };
            }

            var result = googleResponse.Results[0];
            return new GeocodeResult
            {
                IsSuccess = true,
                FormattedAddress = result.FormattedAddress,
                Latitude = result.Geometry.Location.Lat,
                Longitude = result.Geometry.Location.Lng,
                PlaceId = result.PlaceId,
                City = GetAddressComponent(result.AddressComponents, "locality")
                       ?? GetAddressComponent(result.AddressComponents, "administrative_area_level_2"),
                State = GetAddressComponent(result.AddressComponents, "administrative_area_level_1"),
                Country = GetAddressComponent(result.AddressComponents, "country"),
                Pincode = GetAddressComponent(result.AddressComponents, "postal_code")
            };
        }
        catch (Exception ex)
        {
            return new GeocodeResult
            {
                IsSuccess = false,
                ErrorMessage = $"Error during geocoding: {ex.Message}"
            };
        }
    }

    public async Task<GeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude)
    {
        // If no API key is configured, use fallback mock data for development
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            return GetMockReverseGeocodeResult(latitude, longitude);
        }

        try
        {
            var url = $"{_settings.BaseUrl}/geocode/json?latlng={latitude},{longitude}&key={_settings.ApiKey}";

            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            var googleResponse = JsonSerializer.Deserialize<GoogleGeocodeResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (googleResponse?.Status != "OK" || googleResponse.Results == null || googleResponse.Results.Length == 0)
            {
                return new GeocodeResult
                {
                    IsSuccess = false,
                    ErrorMessage = $"Reverse geocoding failed: {googleResponse?.Status}"
                };
            }

            var result = googleResponse.Results[0];
            return new GeocodeResult
            {
                IsSuccess = true,
                FormattedAddress = result.FormattedAddress,
                Latitude = latitude,
                Longitude = longitude,
                PlaceId = result.PlaceId,
                City = GetAddressComponent(result.AddressComponents, "locality")
                       ?? GetAddressComponent(result.AddressComponents, "administrative_area_level_2"),
                State = GetAddressComponent(result.AddressComponents, "administrative_area_level_1"),
                Country = GetAddressComponent(result.AddressComponents, "country"),
                Pincode = GetAddressComponent(result.AddressComponents, "postal_code")
            };
        }
        catch (Exception ex)
        {
            return new GeocodeResult
            {
                IsSuccess = false,
                ErrorMessage = $"Error during reverse geocoding: {ex.Message}"
            };
        }
    }

    public async Task<bool> IsPointInServiceAreaAsync(double latitude, double longitude, int vendorId)
    {
        // Get vendor service areas by pincode matching
        // For full geofencing support, implement GeoJSON polygon checking
        var geocodeResult = await ReverseGeocodeAsync(latitude, longitude);
        if (geocodeResult == null || !geocodeResult.IsSuccess || string.IsNullOrEmpty(geocodeResult.Pincode))
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

    private static string? GetAddressComponent(AddressComponent[]? components, string type)
    {
        return components?
            .FirstOrDefault(c => c.Types.Contains(type))?
            .LongName;
    }

    private static GeocodeResult GetMockGeocodeResult(string address)
    {
        return new GeocodeResult
        {
            IsSuccess = true,
            FormattedAddress = address,
            Latitude = 12.9716, // Bangalore coordinates as default
            Longitude = 77.5946,
            City = "Bangalore",
            State = "Karnataka",
            Country = "India",
            Pincode = "560001"
        };
    }

    private static GeocodeResult GetMockReverseGeocodeResult(double latitude, double longitude)
    {
        return new GeocodeResult
        {
            IsSuccess = true,
            FormattedAddress = "Mock Address, Bangalore, Karnataka 560001, India",
            Latitude = latitude,
            Longitude = longitude,
            City = "Bangalore",
            State = "Karnataka",
            Country = "India",
            Pincode = "560001"
        };
    }
}

// Google Maps API Response Models
internal class GoogleGeocodeResponse
{
    public string Status { get; set; } = string.Empty;
    public GoogleResult[]? Results { get; set; }
}

internal class GoogleResult
{
    public string FormattedAddress { get; set; } = string.Empty;
    public string PlaceId { get; set; } = string.Empty;
    public Geometry Geometry { get; set; } = new();
    public AddressComponent[] AddressComponents { get; set; } = Array.Empty<AddressComponent>();
}

internal class Geometry
{
    public Location Location { get; set; } = new();
}

internal class Location
{
    public double Lat { get; set; }
    public double Lng { get; set; }
}

internal class AddressComponent
{
    public string LongName { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string[] Types { get; set; } = Array.Empty<string>();
}
