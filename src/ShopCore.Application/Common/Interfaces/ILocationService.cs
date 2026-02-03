namespace ShopCore.Application.Common.Interfaces;

public interface ILocationService
{
    Task<GeocodeResult?> GeocodeAddressAsync(string address);
    Task<GeocodeResult?> ReverseGeocodeAsync(double latitude, double longitude);
    Task<bool> IsPointInServiceAreaAsync(double latitude, double longitude, int vendorId);
    Task<double> CalculateDistanceAsync(double lat1, double lon1, double lat2, double lon2);
}

public class GeocodeResult
{
    public bool IsSuccess { get; set; }
    public string? FormattedAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? PlaceId { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Pincode { get; set; }
    public string? ErrorMessage { get; set; }
}
