using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetPendingPayoutAmount;

public class GetPendingPayoutAmountQueryHandler : IRequestHandler<GetPendingPayoutAmountQuery, PendingPayoutDto>
{
    public Task<PendingPayoutDto> Handle(
        GetPendingPayoutAmountQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
