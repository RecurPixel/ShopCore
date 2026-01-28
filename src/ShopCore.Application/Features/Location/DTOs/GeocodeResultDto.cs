namespace ShopCore.Application.Location.DTOs;

public record GeocodeResultDto(
    double Latitude,
    double Longitude,
    string FormattedAddress,
    string? PlaceId,
    string? City,
    string? State,
    string? Country,
    string? PinCode
);
