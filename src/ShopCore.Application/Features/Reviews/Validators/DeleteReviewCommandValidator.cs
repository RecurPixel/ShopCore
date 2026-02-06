using ShopCore.Application.Reviews.Commands.DeleteReview;

namespace ShopCore.Application.Reviews.Validators;

public class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID is required");
    }
}
