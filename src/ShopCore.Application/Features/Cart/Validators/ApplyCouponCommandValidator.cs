using ShopCore.Application.Cart.Commands.ApplyCoupon;

namespace ShopCore.Application.Cart.Validators;

public class ApplyCouponCommandValidator : AbstractValidator<ApplyCouponCommand>
{
    public ApplyCouponCommandValidator()
    {
        RuleFor(x => x.CouponCode)
            .NotEmpty()
            .WithMessage("Coupon code is required")
            .MaximumLength(50)
            .WithMessage("Coupon code cannot exceed 50 characters");
    }
}
