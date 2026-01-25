namespace ShopCore.Application.Coupons.Commands.DeactivateCoupon;

public class DeactivateCouponCommandHandler : IRequestHandler<DeactivateCouponCommand>
{
    public Task Handle(DeactivateCouponCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
