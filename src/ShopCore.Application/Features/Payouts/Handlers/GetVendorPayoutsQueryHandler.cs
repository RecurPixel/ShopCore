namespace ShopCore.Application.Payouts.Queries.GetVendorPayouts;

public class GetVendorPayoutsQueryHandler : IRequestHandler<GetVendorPayoutsQuery, object>
{
    public Task<object> Handle(GetVendorPayoutsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
