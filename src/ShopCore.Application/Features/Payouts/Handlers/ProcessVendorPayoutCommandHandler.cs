namespace ShopCore.Application.Payouts.Commands.ProcessVendorPayout;

public class ProcessVendorPayoutCommandHandler : IRequestHandler<ProcessVendorPayoutCommand>
{
    public Task Handle(ProcessVendorPayoutCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
