using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.ApplyCoupon;

public class ApplyCouponCommandHandler : IRequestHandler<ApplyCouponCommand, CartDto>
{
    public Task<CartDto> Handle(
        ApplyCouponCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        throw new NotImplementedException();
    }
}
