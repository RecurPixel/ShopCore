using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Wishlist.Commands.MoveToCart;

public class MoveToCartCommandHandler : IRequestHandler<MoveToCartCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public MoveToCartCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(MoveToCartCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user from _currentUser service
        // 2. Find product in user's wishlist
        // 3. Check if product is available for purchase
        // 4. Add product to cart with default quantity (1)
        // 5. Remove product from wishlist
        // 6. Fetch and return updated cart
        return new CartDto();
    }
}
