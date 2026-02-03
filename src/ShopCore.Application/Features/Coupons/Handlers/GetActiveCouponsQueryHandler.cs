using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetActiveCoupons;

public class GetActiveCouponsQueryHandler : IRequestHandler<GetActiveCouponsQuery, List<CouponDto>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveCouponsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CouponDto>> Handle(
        GetActiveCouponsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await _context.Coupons
            .AsNoTracking()
            .Where(c => c.IsActive
                     && c.ValidFrom <= now
                     && c.ValidUntil >= now
                     && (!c.UsageLimit.HasValue || c.UsageCount < c.UsageLimit.Value))
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
                IsActive = c.IsActive
            })
            .ToListAsync(cancellationToken);
    }
}