using ShopCore.Application.Wishlist.Commands.RemoveFromWishlist;

namespace ShopCore.Application.Wishlist.Validators;

public class RemoveFromWishlistCommandValidator : AbstractValidator<RemoveFromWishlistCommand>
{
    public RemoveFromWishlistCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");
    }
}
