using ShopCore.Application.Payouts.DTOs;

namespace ShopCore.Application.Payouts.Queries.GetVendorPayouts;

public class GetVendorPayoutsQueryHandler
    : IRequestHandler<GetVendorPayoutsQuery, List<VendorPayoutDto>>
{
    public Task<List<VendorPayoutDto>> Handle(
        GetVendorPayoutsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        // 1. Get current vendor from context
        // 2. Fetch all payouts for this vendor
        // 3. Filter by status if provided (pending, processed, failed)
        // 4. Filter by date range if provided
        // 5. Include amount breakdown (orders, commission, deductions)
        // 6. Include payment method and bank details
        // 7. Sort by date (newest first) and apply pagination
        // 8. Map to VendorPayoutDto list and return
        return Task.FromResult(new List<VendorPayoutDto>());
    }
}
