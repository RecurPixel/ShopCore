using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetMyPayouts;

public class GetMyPayoutsQueryHandler : IRequestHandler<GetMyPayoutsQuery, PaginatedList<PayoutDto>>
{
    public Task<PaginatedList<PayoutDto>> Handle(
        GetMyPayoutsQuery request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        throw new NotImplementedException();
    }
}
