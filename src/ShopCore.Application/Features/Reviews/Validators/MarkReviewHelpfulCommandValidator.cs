using ShopCore.Application.Reviews.Commands.MarkReviewHelpful;

namespace ShopCore.Application.Reviews.Validators;

public class MarkReviewHelpfulCommandValidator : AbstractValidator<MarkReviewHelpfulCommand>
{
    public MarkReviewHelpfulCommandValidator()
    {
        RuleFor(x => x.ReviewId)
            .GreaterThan(0)
            .WithMessage("Review ID is required");
    }
}
