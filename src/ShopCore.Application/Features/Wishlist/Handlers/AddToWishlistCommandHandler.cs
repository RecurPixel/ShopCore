namespace ShopCore.Application.Wishlist.Commands.AddToWishlist;

public class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AddToWishlistCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(AddToWishlistCommand request, CancellationToken ct)
    {
        var productExists = await _context.Products
            .AnyAsync(p => p.Id == request.ProductId && !p.IsDeleted, ct);

        if (!productExists)
            throw new NotFoundException("Product", request.ProductId);

        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == _currentUser.UserId && w.ProductId == request.ProductId, ct);

        if (!exists)
        {
            var wishlistItem = new Wishlist
            {
                UserId = _currentUser.UserId!.Value,
                ProductId = request.ProductId
            };

            _context.Wishlists.Add(wishlistItem);
            await _context.SaveChangesAsync(ct);
        }
    }
}
