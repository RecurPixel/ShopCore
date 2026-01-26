using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.ProcessVendorPayout;

public class ProcessVendorPayoutCommandHandler
    : IRequestHandler<ProcessVendorPayoutCommand, VendorPayoutDto>
{
    public Task<VendorPayoutDto> Handle(
        ProcessVendorPayoutCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new VendorPayoutDto());
    }
}
