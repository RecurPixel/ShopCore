using ShopCore.Application.Common.Models;
using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetAllPayouts;

public class GetAllPayoutsQueryHandler : IRequestHandler<GetAllPayoutsQuery, PaginatedList<PayoutDto>>
{
    public Task<PaginatedList<PayoutDto>> Handle(GetAllPayoutsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        // 1. Fetch all payouts from database (admin view)
        // 2. Filter by vendor if provided
        // 3. Filter by status if provided (pending, processed, failed)
        // 4. Filter by date range if provided
        // 5. Apply pagination (request.Page, request.PageSize)
        // 6. Sort by date (newest first)
        // 7. Include vendor details and transaction info
        // 8. Map to PayoutDto list and return PaginatedList
        return Task.FromResult(new PaginatedList<PayoutDto>([], 0, request.Page, request.PageSize));
    }
}
