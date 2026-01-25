using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Queries.GetAllCoupons;

public record GetAllCouponsQuery : IRequest<List<CouponDto>>;
