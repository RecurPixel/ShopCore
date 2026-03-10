using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetActiveCoupons;

public record GetActiveCouponsQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<CouponDto>>;
