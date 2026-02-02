using ShopCore.Application.Coupons.Commands.UpdateCoupon;
using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Handlers;

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, CouponDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CouponDto> Handle(UpdateCouponCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can update coupons");

        var coupon = await _context.Coupons.FindAsync(request.Id);
        if (coupon == null)
            throw new NotFoundException("Coupon", request.Id);

        coupon.Description = request.Description;
        coupon.DiscountPercentage = request.DiscountPercentage;
        coupon.DiscountAmount = request.DiscountAmount;
        coupon.MinOrderValue = request.MinOrderValue;
        coupon.MaxDiscount = request.MaxDiscount;
        coupon.ValidFrom = request.ValidFrom;
        coupon.ValidUntil = request.ValidUntil;
        coupon.UsageLimit = request.UsageLimit;
        coupon.UsageLimitPerUser = request.UsageLimitPerUser;

        await _context.SaveChangesAsync(ct);

        return new CouponDto { Id = coupon.Id, Code = coupon.Code };
    }
}
