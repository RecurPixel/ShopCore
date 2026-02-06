using ShopCore.Application.Coupons.Commands.DeactivateCoupon;

namespace ShopCore.Application.Coupons.Validators;

public class DeactivateCouponCommandValidator : AbstractValidator<DeactivateCouponCommand>
{
    public DeactivateCouponCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Coupon ID is required");
    }
}
