namespace ShopCore.Application.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, object>
{
    public Task<object> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement query logic
        return Task.FromResult<object>(new { });
    }
}
