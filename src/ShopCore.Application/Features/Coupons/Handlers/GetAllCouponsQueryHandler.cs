using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, List<CouponDto>>
{
    public Task<List<CouponDto>> Handle(
        GetAllCouponsQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        return Task.FromResult<List<CouponDto>>(new List<CouponDto>());
    }
}
