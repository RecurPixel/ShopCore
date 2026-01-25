namespace ShopCore.Application.AdminDashboard.Queries.GetRevenueStats;

public class GetRevenueStatsQueryHandler : IRequestHandler<GetRevenueStatsQuery, object>
{
    public Task<object> Handle(GetRevenueStatsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
