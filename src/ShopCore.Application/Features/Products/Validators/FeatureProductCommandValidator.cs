using ShopCore.Application.Products.Commands.FeatureProduct;

namespace ShopCore.Application.Products.Validators;

public class FeatureProductCommandValidator : AbstractValidator<FeatureProductCommand>
{
    public FeatureProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");
    }
}
