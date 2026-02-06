using ShopCore.Application.Cart.Commands.RemoveCartItem;

namespace ShopCore.Application.Cart.Validators;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId)
            .GreaterThan(0)
            .WithMessage("Cart item ID is required");
    }
}
