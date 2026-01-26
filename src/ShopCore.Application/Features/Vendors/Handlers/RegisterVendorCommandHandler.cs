using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.RegisterVendor;

public class RegisterVendorCommandHandler : IRequestHandler<RegisterVendorCommand, VendorProfileDto>
{
    public Task<VendorProfileDto> Handle(
        RegisterVendorCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new VendorProfileDto());
    }
}
