using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.UpdateCoupon;

public record UpdateCouponCommand(
    int Id,
    string Code,
    string DiscountType,
    decimal DiscountValue,
    decimal? MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? UsageLimit,
    DateTime? StartDate,
    DateTime? EndDate,
    bool IsActive) : IRequest<CouponDto>;
