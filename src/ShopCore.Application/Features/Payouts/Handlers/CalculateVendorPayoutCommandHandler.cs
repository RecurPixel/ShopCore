using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

public class CalculateVendorPayoutCommandHandler
    : IRequestHandler<CalculateVendorPayoutCommand, VendorPayoutDto>
{
    public Task<VendorPayoutDto> Handle(
        CalculateVendorPayoutCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new VendorPayoutDto());
    }
}
