using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.CreateAddress;

public record CreateAddressCommand(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Country,
    string Pincode,
    bool IsDefault,
    // Location fields
    double? Latitude,
    double? Longitude,
    string? PlaceId,
    AddressType Type,
    string? Landmark
) : IRequest<AddressDto>;
