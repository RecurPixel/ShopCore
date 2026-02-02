namespace ShopCore.Application.Coupons.Commands.DeactivateCoupon;

public class DeactivateCouponCommandHandler : IRequestHandler<DeactivateCouponCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeactivateCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeactivateCouponCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can deactivate coupons");

        var coupon = await _context.Coupons.FindAsync(request.Id);
        if (coupon == null)
            throw new NotFoundException("Coupon", request.Id);

        coupon.IsActive = false;
        await _context.SaveChangesAsync(ct);
    }
}
