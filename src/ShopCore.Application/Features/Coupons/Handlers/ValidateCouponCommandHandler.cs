using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.ValidateCoupon;

public class ValidateCouponCommandHandler
    : IRequestHandler<ValidateCouponCommand, CouponValidationResultDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTime _dateTime;

    public ValidateCouponCommandHandler(IApplicationDbContext context, IDateTime dateTime)
    {
        _context = context;
        _dateTime = dateTime;
    }

    public async Task<CouponValidationResultDto> Handle(ValidateCouponCommand request, CancellationToken ct)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Code == request.CouponCode.ToUpperInvariant(), ct);

        if (coupon == null || !coupon.IsActive)
            return new CouponValidationResultDto { IsValid = false, ErrorMessage = "Invalid coupon code" };

        if (_dateTime.UtcNow < coupon.ValidFrom)
            return new CouponValidationResultDto { IsValid = false, ErrorMessage = "Coupon not yet valid" };

        if (_dateTime.UtcNow > coupon.ValidUntil)
            return new CouponValidationResultDto { IsValid = false, ErrorMessage = "Coupon has expired" };

        if (coupon.UsageLimit.HasValue && coupon.UsageCount >= coupon.UsageLimit)
            return new CouponValidationResultDto { IsValid = false, ErrorMessage = "Coupon usage limit reached" };

        if (coupon.MinOrderValue.HasValue && request.OrderTotal < coupon.MinOrderValue)
            return new CouponValidationResultDto
            {
                IsValid = false,
                ErrorMessage = $"Minimum order value ₹{coupon.MinOrderValue} required"
            };

        // Calculate discount
        decimal discount = 0;
        if (coupon.Type == CouponType.Percentage)
        {
            discount = request.OrderTotal * (coupon.DiscountPercentage!.Value / 100);
            if (coupon.MaxDiscount.HasValue && discount > coupon.MaxDiscount)
                discount = coupon.MaxDiscount.Value;
        }
        else if (coupon.Type == CouponType.FixedAmount)
        {
            discount = coupon.DiscountAmount!.Value;
        }

        return new CouponValidationResultDto
        {
            IsValid = true,
            DiscountAmount = discount,
            Coupon = new CouponDto
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
            }
        };
    }
}
