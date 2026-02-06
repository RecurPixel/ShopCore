using ShopCore.Application.Reviews.Commands.RespondToReview;

namespace ShopCore.Application.Reviews.Validators;

public class RespondToReviewCommandValidator : AbstractValidator<RespondToReviewCommand>
{
    public RespondToReviewCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0)
            .WithMessage("Review ID is required");

        RuleFor(x => x.VendorResponse)
            .NotEmpty()
            .WithMessage("Response is required")
            .MaximumLength(1000)
            .WithMessage("Response cannot exceed 1000 characters");
    }
}
