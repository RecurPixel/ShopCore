namespace ShopCore.Application.Wishlist.Commands.RemoveFromWishlist;

public class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveFromWishlistCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RemoveFromWishlistCommand request, CancellationToken ct)
    {
        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == _currentUser.UserId && w.ProductId == request.ProductId, ct);

        if (wishlistItem != null)
        {
            _context.Wishlists.Remove(wishlistItem);
            await _context.SaveChangesAsync(ct);
        }
    }
}
