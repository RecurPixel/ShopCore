using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Commands.UpdateVendorOrderStatus;

public class UpdateVendorOrderStatusCommandHandler
    : IRequestHandler<UpdateVendorOrderStatusCommand, VendorOrderDto>
{
    public Task<VendorOrderDto> Handle(
        UpdateVendorOrderStatusCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new VendorOrderDto());
    }
}
