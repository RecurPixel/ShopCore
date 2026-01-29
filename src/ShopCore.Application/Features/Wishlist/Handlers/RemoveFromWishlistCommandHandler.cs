namespace ShopCore.Application.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand>
{
    public Task Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from context
        // 2. Find wishlist item for the product
        // 3. Remove item from wishlist
        // 4. Save changes to database
        // 5. Return success
        return Task.CompletedTask;
    }
}
