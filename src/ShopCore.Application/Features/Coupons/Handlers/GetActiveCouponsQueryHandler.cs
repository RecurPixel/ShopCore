namespace ShopCore.Application.Coupons.Queries.GetActiveCoupons;

public class GetActiveCouponsQueryHandler : IRequestHandler<GetActiveCouponsQuery, object>
{
    public Task<object> Handle(GetActiveCouponsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
