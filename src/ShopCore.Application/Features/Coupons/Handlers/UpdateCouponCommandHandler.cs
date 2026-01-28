using ShopCore.Application.Coupons.Commands.UpdateCoupon;
using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Handlers;

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, CouponDto>
{
    public Task<CouponDto> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
