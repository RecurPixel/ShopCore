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
        // 1. Get current user's wishlist
        // 2. Find product in wishlist
        // 3. Add product to cart
        // 4. Remove product from wishlist
        // 5. Return updated cart
        return new CartDto();
    }
}
