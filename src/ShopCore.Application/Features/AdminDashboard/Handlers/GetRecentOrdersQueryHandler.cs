namespace ShopCore.Application.AdminDashboard.Queries.GetRecentOrders;

public class GetRecentOrdersQueryHandler : IRequestHandler<GetRecentOrdersQuery, object>
{
    public Task<object> Handle(GetRecentOrdersQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
