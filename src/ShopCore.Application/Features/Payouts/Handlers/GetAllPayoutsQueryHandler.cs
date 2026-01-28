using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetAllPayouts;

public class GetAllPayoutsQueryHandler : IRequestHandler<GetAllPayoutsQuery, PaginatedList<PayoutDto>>
{
    public Task<PaginatedList<PayoutDto>> Handle(GetAllPayoutsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
