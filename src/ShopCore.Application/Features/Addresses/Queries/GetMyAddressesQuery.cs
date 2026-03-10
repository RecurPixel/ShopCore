using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetMyAddresses;

public record GetMyAddressesQuery(
    string? Search = null,
    bool? IsDefault = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<AddressDto>>;
