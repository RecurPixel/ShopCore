using ShopCore.Application.Cart.Commands.AddCartItem;

namespace ShopCore.Application.Cart.Validators;

public class AddCartItemCommandValidator : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Invalid product ID");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be at least 1")
            .LessThanOrEqualTo(100)
            .WithMessage("Quantity cannot exceed 100");
    }
}
