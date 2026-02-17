using ShopCore.Application.Coupons.DTOs;

namespace ShopCore.Application.Coupons.Commands.UpdateCoupon;

// Note: IsActive is controlled via ActivateCouponCommand/DeactivateCouponCommand
public record UpdateCouponCommand(
    int Id,
    string Code,
    string DiscountType,
    decimal DiscountValue,
    decimal? MinOrderAmount,
    decimal? MaxDiscountAmount,
    int? UsageLimit,
    DateTime? StartDate,
    DateTime? EndDate) : IRequest<CouponDto>;
