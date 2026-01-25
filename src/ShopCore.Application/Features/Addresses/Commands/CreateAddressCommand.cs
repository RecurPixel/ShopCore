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
    string PinCode,
    bool IsDefault
) : IRequest<AddressDto>;
