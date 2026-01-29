using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CreatePayout;

public class CreatePayoutCommandHandler : IRequestHandler<CreatePayoutCommand, PayoutDto>
{
    public Task<PayoutDto> Handle(CreatePayoutCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current vendor from context
        // 2. Calculate available balance from pending orders
        // 3. Validate requested payout amount doesn't exceed balance
        // 4. Create payout record with status 'pending'
        // 5. Store vendor bank account details if provided
        // 6. Create transaction entry
        // 7. Map and return PayoutDto with confirmation
        return Task.FromResult(default(PayoutDto));
    }
}
