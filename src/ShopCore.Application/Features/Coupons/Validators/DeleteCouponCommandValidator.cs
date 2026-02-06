using ShopCore.Application.Coupons.Commands.DeleteCoupon;

namespace ShopCore.Application.Coupons.Validators;

public class DeleteCouponCommandValidator : AbstractValidator<DeleteCouponCommand>
{
    public DeleteCouponCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Coupon ID is required");
    }
}
