using ShopCore.Application.Vendors.DTOs;

namespace ShopCore.Application.Vendors.Queries.GetMyVendorStats;

public class GetMyVendorStatsQueryHandler : IRequestHandler<GetMyVendorStatsQuery, VendorStatsDto>
{
    public Task<VendorStatsDto> Handle(
        GetMyVendorStatsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult(new VendorStatsDto());
    }
}
