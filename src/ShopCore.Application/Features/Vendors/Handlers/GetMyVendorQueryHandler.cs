namespace ShopCore.Application.Vendors.Queries.GetMyVendor;

public class GetMyVendorQueryHandler : IRequestHandler<GetMyVendorQuery, object>
{
    public Task<object> Handle(GetMyVendorQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
