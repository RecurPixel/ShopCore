using ShopCore.Application.Coupons.Commands.DeleteCoupon;

namespace ShopCore.Application.Coupons.Handlers;

public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteCouponCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can delete coupons");

        var coupon = await _context.Coupons.FindAsync(request.Id);
        if (coupon == null)
            throw new NotFoundException("Coupon", request.Id);

        coupon.IsDeleted = true;
        await _context.SaveChangesAsync(ct);
    }
}
