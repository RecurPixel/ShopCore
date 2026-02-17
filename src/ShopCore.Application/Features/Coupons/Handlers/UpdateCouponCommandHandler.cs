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

        // Parse discount type
        var discountType = request.DiscountType?.ToLowerInvariant() switch
        {
            "percentage" => CouponType.Percentage,
            "fixed" or "fixedamount" => CouponType.FixedAmount,
            _ => coupon.Type // Keep existing if not specified
        };

        coupon.Code = request.Code;
        coupon.Type = discountType;

        // Set discount value based on type
        if (discountType == CouponType.Percentage)
        {
            coupon.DiscountPercentage = request.DiscountValue;
            coupon.DiscountAmount = null;
        }
        else
        {
            coupon.DiscountAmount = request.DiscountValue;
            coupon.DiscountPercentage = null;
        }

        coupon.MinOrderValue = request.MinOrderAmount;
        coupon.MaxDiscount = request.MaxDiscountAmount;
        coupon.ValidFrom = request.StartDate ?? coupon.ValidFrom;
        coupon.ValidUntil = request.EndDate ?? coupon.ValidUntil;
        coupon.UsageLimit = request.UsageLimit;
        // Note: IsActive is controlled via ActivateCouponCommand/DeactivateCouponCommand

        await _context.SaveChangesAsync(ct);

        return new CouponDto
        {
            Id = coupon.Id,
            Code = coupon.Code,
            Description = coupon.Description,
            Type = coupon.Type.ToString(),
            DiscountPercentage = coupon.DiscountPercentage,
            DiscountAmount = coupon.DiscountAmount,
            MinOrderValue = coupon.MinOrderValue,
            MaxDiscount = coupon.MaxDiscount,
            ValidFrom = coupon.ValidFrom,
            ValidUntil = coupon.ValidUntil,
            UsageLimit = coupon.UsageLimit,
            UsageCount = coupon.UsageCount,
            UsageLimitPerUser = coupon.UsageLimitPerUser,
            IsActive = coupon.IsActive,
            IsValid = coupon.IsValid
        };
    }
}
