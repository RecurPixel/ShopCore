using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetActiveCoupons;

public class GetActiveCouponsQueryHandler : IRequestHandler<GetActiveCouponsQuery, List<CouponDto>>
{
    public Task<List<CouponDto>> Handle(
        GetActiveCouponsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<List<CouponDto>>(new List<CouponDto>());
    }
}
