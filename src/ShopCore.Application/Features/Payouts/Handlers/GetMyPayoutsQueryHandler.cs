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
        // 1. Get current vendor from context
        // 2. Fetch all payouts for this vendor
        // 3. Filter by status if provided (pending, processed, failed)
        // 4. Filter by date range if provided
        // 5. Apply pagination (request.Page, request.PageSize)
        // 6. Sort by date (newest first)
        // 7. Map to PayoutDto list
        // 8. Return PaginatedList<PayoutDto>
        return Task.FromResult(new PaginatedList<PayoutDto>([], 0, request.Page, request.PageSize));
    }
}
