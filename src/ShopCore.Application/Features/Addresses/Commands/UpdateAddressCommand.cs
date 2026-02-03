using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.UpdateAddress;

public record UpdateAddressCommand(
    int Id,
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Country,
    string Pincode,
    bool IsDefault
) : IRequest<AddressDto>;
