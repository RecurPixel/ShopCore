using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetActiveCoupons;

public class GetActiveCouponsQueryHandler : IRequestHandler<GetActiveCouponsQuery, PaginatedList<CouponDto>>
{
    private readonly IApplicationDbContext _context;

    public GetActiveCouponsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CouponDto>> Handle(
        GetActiveCouponsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;


        var query = _context.Coupons
            .AsNoTracking()
            .Where(c => c.IsActive
                     && c.ValidFrom <= now
                     && c.ValidUntil >= now
                     && (!c.UsageLimit.HasValue || c.UsageCount < c.UsageLimit.Value));

        // Apply search filter
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(c => c.Code.Contains(request.Search) || c.Description.Contains(request.Search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(c => c.ValidUntil)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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


        return new PaginatedList<CouponDto>
        {
            Items = items,
            TotalItems = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}