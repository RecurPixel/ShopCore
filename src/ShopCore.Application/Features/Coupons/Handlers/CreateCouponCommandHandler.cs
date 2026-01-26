using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, CouponDto>
{
    public Task<CouponDto> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.FromResult(new CouponDto());
    }
}
