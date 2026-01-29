namespace ShopCore.Application.Payouts.Commands.CancelPayout;

public class CancelPayoutCommandHandler : IRequestHandler<CancelPayoutCommand>
{
    public Task Handle(CancelPayoutCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get payout by id
        // 2. Verify vendor owns this payout
        // 3. Ensure payout status is 'pending' (only pending can be cancelled)
        // 4. Update status to 'cancelled'
        // 5. Log cancellation reason
        // 6. Send notification to vendor
        // 7. Update available balance if needed
        return Task.CompletedTask;
    }
}
