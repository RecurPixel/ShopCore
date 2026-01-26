using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetAddressById;

public record GetAddressByIdQuery(int Id) : IRequest<AddressDto>;
