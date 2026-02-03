using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCoupon;

public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, CartDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RemoveCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CartDto> Handle(RemoveCouponCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart != null)
        {
            cart.AppliedCouponCode = null;
            cart.Discount = null;
            await _context.SaveChangesAsync(ct);
        }

        return new CartDto { Id = cart?.Id ?? 0 };
    }
}
