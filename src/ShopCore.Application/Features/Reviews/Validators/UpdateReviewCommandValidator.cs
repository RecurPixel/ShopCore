using ShopCore.Application.Reviews.Commands.UpdateReview;

namespace ShopCore.Application.Reviews.Validators;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Review ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Title)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .WithMessage("Comment is required")
            .MaximumLength(2000)
            .WithMessage("Comment cannot exceed 2000 characters");
    }
}
