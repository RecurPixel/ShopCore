namespace ShopCore.Application.Location.DTOs;

public record ReverseGeocodeResultDto(
    string FormattedAddress,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? State,
    string? Country,
    string? PinCode,
    string? PlaceId
);
