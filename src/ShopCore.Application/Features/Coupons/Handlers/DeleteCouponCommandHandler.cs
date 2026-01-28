using ShopCore.Application.Coupons.Commands.DeleteCoupon;

namespace ShopCore.Application.Coupons.Handlers;

public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand>
{
    public Task Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
