using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetAllCoupons;

public record GetAllCouponsQuery(
    string? Search = null,
    string? Status = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<CouponDto>>;
