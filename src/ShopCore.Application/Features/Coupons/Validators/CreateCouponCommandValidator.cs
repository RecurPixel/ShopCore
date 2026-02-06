using ShopCore.Application.Coupons.Commands.CreateCoupon;

namespace ShopCore.Application.Coupons.Validators;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Coupon code is required")
            .MaximumLength(50)
            .WithMessage("Coupon code cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid coupon type");

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue)
            .WithMessage("Discount percentage must be between 0 and 100");

        RuleFor(x => x.DiscountAmount)
            .GreaterThan(0)
            .When(x => x.DiscountAmount.HasValue)
            .WithMessage("Discount amount must be greater than 0");

        RuleFor(x => x.MinOrderValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinOrderValue.HasValue)
            .WithMessage("Minimum order value cannot be negative");

        RuleFor(x => x.MaxDiscount)
            .GreaterThan(0)
            .When(x => x.MaxDiscount.HasValue)
            .WithMessage("Maximum discount must be greater than 0");

        RuleFor(x => x.ValidFrom)
            .LessThan(x => x.ValidUntil)
            .WithMessage("Valid from date must be before valid until date");

        RuleFor(x => x.UsageLimit)
            .GreaterThan(0)
            .When(x => x.UsageLimit.HasValue)
            .WithMessage("Usage limit must be greater than 0");

        RuleFor(x => x.UsageLimitPerUser)
            .GreaterThan(0)
            .When(x => x.UsageLimitPerUser.HasValue)
            .WithMessage("Usage limit per user must be greater than 0");
    }
}
