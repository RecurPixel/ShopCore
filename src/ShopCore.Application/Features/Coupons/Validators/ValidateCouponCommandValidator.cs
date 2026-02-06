using ShopCore.Application.Coupons.Commands.ValidateCoupon;

namespace ShopCore.Application.Coupons.Validators;

public class ValidateCouponCommandValidator : AbstractValidator<ValidateCouponCommand>
{
    public ValidateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Coupon code is required");

        RuleFor(x => x.CartTotal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cart total cannot be negative");
    }
}
