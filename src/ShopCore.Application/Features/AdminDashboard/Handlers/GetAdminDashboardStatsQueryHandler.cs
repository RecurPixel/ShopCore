namespace ShopCore.Application.AdminDashboard.Queries.GetAdminDashboardStats;

public class GetAdminDashboardStatsQueryHandler
    : IRequestHandler<GetAdminDashboardStatsQuery, object>
{
    public Task<object> Handle(
        GetAdminDashboardStatsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
