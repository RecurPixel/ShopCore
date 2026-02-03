using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, List<CouponDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCouponsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CouponDto>> Handle(
        GetAllCouponsQuery request,
        CancellationToken cancellationToken)
    {
        return await _context.Coupons
            .AsNoTracking()
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CouponDto
            {
                Id = c.Id,
                Code = c.Code,
                Description = c.Description,
                Type = c.Type.ToString(),
                DiscountPercentage = c.DiscountPercentage,
                DiscountAmount = c.DiscountAmount,
                MinOrderValue = c.MinOrderValue,
                MaxDiscount = c.MaxDiscount,
                ValidFrom = c.ValidFrom,
                ValidUntil = c.ValidUntil,
                UsageLimit = c.UsageLimit,
                UsageCount = c.UsageCount,
                UsageLimitPerUser = c.UsageLimitPerUser,
                IsActive = c.IsActive,
                IsValid = c.IsValid
            })
            .ToListAsync(cancellationToken);
    }
}
