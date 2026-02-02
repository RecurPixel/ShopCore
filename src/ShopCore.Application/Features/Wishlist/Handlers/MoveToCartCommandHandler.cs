using ShopCore.Application.Cart.Commands.AddCartItem;
using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Wishlist.Commands.MoveToCart;

public class MoveToCartCommandHandler : IRequestHandler<MoveToCartCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public MoveToCartCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<CartDto> Handle(MoveToCartCommand request, CancellationToken ct)
    {
        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == _currentUser.UserId && w.ProductId == request.ProductId, ct);

        if (wishlistItem == null)
            throw new NotFoundException("Product not in wishlist");

        // Add to cart
        var cartDto = await _mediator.Send(new AddCartItemCommand(request.ProductId, 1), ct);

        // Remove from wishlist
        _context.Wishlists.Remove(wishlistItem);
        await _context.SaveChangesAsync(ct);

        return cartDto;
    }
}
