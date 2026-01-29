using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCoupon;

public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, CartDto>
{
    public Task<CartDto> Handle(
        RemoveCouponCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get current user's cart
        // 2. Verify coupon is applied to this cart
        // 3. Remove coupon from cart
        // 4. Recalculate cart total (remove discount)
        // 5. Recalculate taxes if applicable
        // 6. Update cart in database
        // 7. Map and return updated CartDto
        return Task.FromResult(default(CartDto));
    }
}
