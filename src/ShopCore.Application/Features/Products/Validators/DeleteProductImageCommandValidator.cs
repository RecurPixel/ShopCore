using ShopCore.Application.Products.Commands.DeleteProductImage;

namespace ShopCore.Application.Products.Validators;

public class DeleteProductImageCommandValidator : AbstractValidator<DeleteProductImageCommand>
{
    public DeleteProductImageCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");

        RuleFor(x => x.ImageId)
            .GreaterThan(0)
            .WithMessage("Image ID is required");
    }
}
