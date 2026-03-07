namespace ShopCore.Infrastructure.Services;

public class GoogleMapsSettings
{
    public string? ApiKey { get; set; }
    public string BaseUrl { get; set; } = "https://maps.googleapis.com/maps/api";
}
