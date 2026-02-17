using ShopCore.Application.Coupons.Commands.ActivateCoupon;

namespace ShopCore.Application.Coupons.Validators;

public class ActivateCouponCommandValidator : AbstractValidator<ActivateCouponCommand>
{
    public ActivateCouponCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Coupon ID is required");
    }
}
