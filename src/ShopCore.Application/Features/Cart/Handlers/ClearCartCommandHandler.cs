namespace ShopCore.Application.Cart.Commands.ClearCart;

public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ClearCartCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        var cart = await _context.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == _currentUser.UserId, ct);

        if (cart != null)
        {
            foreach (var item in cart.Items)
                item.IsDeleted = true;

            cart.AppliedCouponCode = null;
            cart.Discount = null;

            await _context.SaveChangesAsync(ct);
        }
    }
}
