using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Commands.CalculatePendingPayouts;

public class CalculatePendingPayoutsCommandHandler : IRequestHandler<CalculatePendingPayoutsCommand, PendingPayoutSummaryDto>
{
    public Task<PendingPayoutSummaryDto> Handle(CalculatePendingPayoutsCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
