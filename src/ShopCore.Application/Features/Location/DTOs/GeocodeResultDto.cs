namespace ShopCore.Application.Location.DTOs;

public record GeocodeResultDto
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string FormattedAddress { get; init; } = string.Empty;
    public string? PlaceId { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? PinCode { get; init; }
}