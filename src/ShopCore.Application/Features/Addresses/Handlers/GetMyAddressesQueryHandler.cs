using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetMyAddresses;

public class GetMyAddressesQueryHandler : IRequestHandler<GetMyAddressesQuery, List<AddressDto>>
{
    public Task<List<AddressDto>> Handle(
        GetMyAddressesQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new List<AddressDto>());
    }
}
