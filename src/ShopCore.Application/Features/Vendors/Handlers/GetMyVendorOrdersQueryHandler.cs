namespace ShopCore.Application.Vendors.Queries.GetMyVendorOrders;

public class GetMyVendorOrdersQueryHandler : IRequestHandler<GetMyVendorOrdersQuery, object>
{
    public Task<object> Handle(GetMyVendorOrdersQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
