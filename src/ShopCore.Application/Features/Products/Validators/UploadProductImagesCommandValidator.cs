using ShopCore.Application.Products.Commands.UploadProductImages;

namespace ShopCore.Application.Products.Validators;

public class UploadProductImagesCommandValidator : AbstractValidator<UploadProductImagesCommand>
{
    public UploadProductImagesCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");

        RuleFor(x => x.Images)
            .NotEmpty()
            .WithMessage("At least one image is required");
    }
}
