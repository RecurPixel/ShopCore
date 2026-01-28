using ShopCore.Application.Cart.DTOs;

namespace ShopCore.Application.Cart.Commands.RemoveCoupon;

public class RemoveCouponCommandHandler : IRequestHandler<RemoveCouponCommand, CartDto>
{
    public Task<CartDto> Handle(
        RemoveCouponCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        throw new NotImplementedException();
    }
}
