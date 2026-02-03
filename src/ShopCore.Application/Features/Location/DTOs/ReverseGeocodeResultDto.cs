namespace ShopCore.Application.Location.DTOs;

public record ReverseGeocodeResultDto
{
    public string FormattedAddress { get; init; } = string.Empty;
    public string? AddressLine1 { get; init; }
    public string? AddressLine2 { get; init; }
    public string? City { get; init; }
    public string? State { get; init; }
    public string? Country { get; init; }
    public string? PinCode { get; init; }
    public string? PlaceId { get; init; }
}