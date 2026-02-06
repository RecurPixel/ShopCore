using ShopCore.Application.Coupons.Commands.UpdateCoupon;

namespace ShopCore.Application.Coupons.Validators;

public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Coupon ID is required");

        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Coupon code is required")
            .MaximumLength(50)
            .WithMessage("Coupon code cannot exceed 50 characters");

        RuleFor(x => x.DiscountType)
            .NotEmpty()
            .WithMessage("Discount type is required");

        RuleFor(x => x.DiscountValue)
            .GreaterThan(0)
            .WithMessage("Discount value must be greater than 0");

        RuleFor(x => x.MinOrderAmount)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinOrderAmount.HasValue)
            .WithMessage("Minimum order amount cannot be negative");

        RuleFor(x => x.MaxDiscountAmount)
            .GreaterThan(0)
            .When(x => x.MaxDiscountAmount.HasValue)
            .WithMessage("Maximum discount amount must be greater than 0");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0)
            .When(x => x.UsageLimit.HasValue)
            .WithMessage("Usage limit must be greater than 0");

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("Start date must be before end date");
    }
}
