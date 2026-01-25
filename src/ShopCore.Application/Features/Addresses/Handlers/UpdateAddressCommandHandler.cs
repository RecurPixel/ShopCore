using ShopCore.Application.Addresses.DTOs;

namespace ShopCore.Application.Addresses.Commands.UpdateAddress;

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, AddressDto>
{
    public Task<AddressDto> Handle(
        UpdateAddressCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new AddressDto());
    }
}
