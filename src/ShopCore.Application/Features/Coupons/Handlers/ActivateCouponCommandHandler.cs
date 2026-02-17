namespace ShopCore.Application.Coupons.Commands.ActivateCoupon;

public class ActivateCouponCommandHandler : IRequestHandler<ActivateCouponCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ActivateCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(ActivateCouponCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can activate coupons");

        var coupon = await _context.Coupons.FindAsync(request.Id);
        if (coupon == null)
            throw new NotFoundException("Coupon", request.Id);

        coupon.IsActive = true;
        await _context.SaveChangesAsync(ct);
    }
}
