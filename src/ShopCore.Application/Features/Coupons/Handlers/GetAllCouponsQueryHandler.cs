using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetAllCoupons;

public class GetAllCouponsQueryHandler : IRequestHandler<GetAllCouponsQuery, PaginatedList<CouponDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllCouponsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<CouponDto>> Handle(
        GetAllCouponsQuery request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query = _context.Coupons.AsNoTracking();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Search))
        {
            query = query.Where(c => c.Code.Contains(request.Search) || (c.Description != null && c.Description.Contains(request.Search)));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(c => c.IsActive == request.IsActive.Value);
        }

        if (!string.IsNullOrEmpty(request.Status))
        {
            query = request.Status.ToLower() switch
            {
                "active" => query.Where(c => c.IsActive && c.ValidFrom <= now && c.ValidUntil >= now),
                "expired" => query.Where(c => c.ValidUntil < now),
                "upcoming" => query.Where(c => c.ValidFrom > now),
                _ => query
            };
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(c => c.CreatedAt)
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
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalCount
        };
    }
}
