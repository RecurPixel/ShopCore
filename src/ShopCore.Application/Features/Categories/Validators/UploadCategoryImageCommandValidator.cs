using ShopCore.Application.Categories.Commands.UploadCategoryImage;

namespace ShopCore.Application.Categories.Validators;

public class UploadCategoryImageCommandValidator : AbstractValidator<UploadCategoryImageCommand>
{
    public UploadCategoryImageCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("Category ID is required");

        RuleFor(x => x.AvatarFile)
            .NotNull()
            .WithMessage("Image file is required");
    }
}
