using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;

public class GetPendingPayoutAmountQueryHandler : IRequestHandler<GetPendingPayoutAmountQuery, PendingPayoutDto>
{
    public Task<PendingPayoutDto> Handle(
        GetPendingPayoutAmountQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Calculate pending orders total (completed but unpaid)
        // 3. Calculate subscription fees pending
        // 4. Subtract commission percentage (if applicable)
        // 5. Subtract any returns or refunds
        // 6. Return PendingPayoutDto with amount and breakdown
        return Task.FromResult(default(PendingPayoutDto));
    }
}
