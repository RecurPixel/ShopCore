using ShopCore.Application.Categories.Commands.DeleteCategory;

namespace ShopCore.Application.Categories.Validators;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Category ID is required");
    }
}
