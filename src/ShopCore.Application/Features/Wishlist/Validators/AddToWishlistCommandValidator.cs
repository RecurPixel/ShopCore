using ShopCore.Application.Wishlist.Commands.AddToWishlist;

namespace ShopCore.Application.Wishlist.Validators;

public class AddToWishlistCommandValidator : AbstractValidator<AddToWishlistCommand>
{
    public AddToWishlistCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("Product ID is required");
    }
}
