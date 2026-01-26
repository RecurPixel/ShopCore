using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Queries.GetAddressById;

public class GetAddressByIdQueryHandler : IRequestHandler<GetAddressByIdQuery, AddressDto>
{
    public Task<AddressDto> Handle(
        GetAddressByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new AddressDto());
    }
}
