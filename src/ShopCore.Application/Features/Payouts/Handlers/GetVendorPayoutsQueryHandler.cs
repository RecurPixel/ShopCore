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
        return Task.FromResult(new List<VendorPayoutDto>());
    }
}
