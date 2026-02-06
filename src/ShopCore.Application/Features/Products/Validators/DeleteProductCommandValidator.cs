using ShopCore.Application.Products.Commands.DeleteProduct;

namespace ShopCore.Application.Products.Validators;

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product ID is required");
    }
}
