using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.UpdateMyVendor;

public class UpdateMyVendorCommandHandler : IRequestHandler<UpdateMyVendorCommand, VendorProfileDto>
{
    public Task<VendorProfileDto> Handle(
        UpdateMyVendorCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new VendorProfileDto());
    }
}
