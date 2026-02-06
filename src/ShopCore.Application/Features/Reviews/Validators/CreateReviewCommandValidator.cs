using ShopCore.Application.Reviews.Commands.CreateReview;

namespace ShopCore.Application.Reviews.Validators;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");

        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID is required");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Title)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Title))
            .WithMessage("Title cannot exceed 100 characters");

        RuleFor(x => x.Comment)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Comment))
            .WithMessage("Comment cannot exceed 2000 characters");
    }
}
