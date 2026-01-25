using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetActiveCoupons;

public record GetActiveCouponsQuery : IRequest<List<CouponDto>>;
