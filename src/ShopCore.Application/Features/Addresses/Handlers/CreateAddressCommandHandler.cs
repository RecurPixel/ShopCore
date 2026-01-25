using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.CreateAddress;

public class CreateAddressCommandHandler : IRequestHandler<CreateAddressCommand, AddressDto>
{
    public Task<AddressDto> Handle(
        CreateAddressCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new AddressDto());
    }
}
