using ShopCore.Application.Categories.Commands.UpdateCategory;

namespace ShopCore.Application.Categories.Validators;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters");

        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug is required")
            .MaximumLength(100)
            .WithMessage("Slug cannot exceed 100 characters")
            .Matches("^[a-z0-9-]+$")
            .WithMessage("Slug can only contain lowercase letters, numbers, and hyphens");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.ParentCategoryId)
            .GreaterThan(0)
            .When(x => x.ParentCategoryId.HasValue)
            .WithMessage("Invalid parent category ID");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display order cannot be negative");
    }
}
