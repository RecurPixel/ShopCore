namespace ShopCore.Application.Vendors.Queries.GetMyVendorStats;

public class GetMyVendorStatsQueryHandler : IRequestHandler<GetMyVendorStatsQuery, object>
{
    public Task<object> Handle(GetMyVendorStatsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
