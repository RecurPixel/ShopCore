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

    public Task Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from _currentUser service
        // 2. Check if product exists and is available
        // 3. Check if product is already in wishlist
        // 4. Create wishlist item if not exists
        // 5. Save changes to database
        // 6. Return success
        return Task.CompletedTask;
    }
}
