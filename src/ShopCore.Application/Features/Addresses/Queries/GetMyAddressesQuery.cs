using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetMyAddresses;

public record GetMyAddressesQuery : IRequest<List<AddressDto>>;
