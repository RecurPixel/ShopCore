namespace ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

public class CalculateVendorPayoutCommandHandler : IRequestHandler<CalculateVendorPayoutCommand>
{
    public Task Handle(CalculateVendorPayoutCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
