using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.ValidateCoupon;

public class ValidateCouponCommandHandler
    : IRequestHandler<ValidateCouponCommand, CouponValidationResultDto>
{
    public Task<CouponValidationResultDto> Handle(
        ValidateCouponCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new CouponValidationResultDto());
    }
}
