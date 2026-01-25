namespace ShopCore.Application.Coupons.Commands.DeactivateCoupon;

public record DeactivateCouponCommand(int Id) : IRequest<Unit>;
