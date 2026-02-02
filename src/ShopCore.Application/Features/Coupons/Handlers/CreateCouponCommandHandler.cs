using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, CouponDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCouponCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<CouponDto> Handle(CreateCouponCommand request, CancellationToken ct)
    {
        // Admin only
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can create coupons");

        // Verify code is unique
        var exists = await _context.Coupons
            .AnyAsync(c => c.Code == request.Code.ToUpperInvariant(), ct);

        if (exists)
            throw new ValidationException("Coupon code already exists");

        var coupon = new Coupon
        {
            Code = request.Code.ToUpperInvariant(),
            Description = request.Description,
            Type = request.Type,
            DiscountPercentage = request.DiscountPercentage,
            DiscountAmount = request.DiscountAmount,
            MinOrderValue = request.MinOrderValue,
            MaxDiscount = request.MaxDiscount,
            ValidFrom = request.ValidFrom,
            ValidUntil = request.ValidUntil,
            UsageLimit = request.UsageLimit,
            UsageLimitPerUser = request.UsageLimitPerUser,
            UsageCount = 0,
            IsActive = request.IsActive
        };

        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync(ct);

        return new CouponDto { Id = coupon.Id, Code = coupon.Code, Type = coupon.Type.ToString() };
    }
}
