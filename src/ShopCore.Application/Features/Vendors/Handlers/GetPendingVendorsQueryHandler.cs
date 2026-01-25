namespace ShopCore.Application.Vendors.Queries.GetPendingVendors;

public class GetPendingVendorsQueryHandler : IRequestHandler<GetPendingVendorsQuery, object>
{
    public Task<object> Handle(GetPendingVendorsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
