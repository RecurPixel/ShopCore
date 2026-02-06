using ShopCore.Application.Wishlist.Commands.MoveToCart;

namespace ShopCore.Application.Wishlist.Validators;

public class MoveToCartCommandValidator : AbstractValidator<MoveToCartCommand>
{
    public MoveToCartCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");
    }
}
