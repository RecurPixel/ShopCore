using ShopCore.Application.Products.Commands.UpdateProductStatus;

namespace ShopCore.Application.Products.Validators;

public class UpdateProductStatusCommandValidator : AbstractValidator<UpdateProductStatusCommand>
{
    public UpdateProductStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Product ID is required");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid product status");
    }
}
