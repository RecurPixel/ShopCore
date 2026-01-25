namespace ShopCore.Application.Coupons.Commands.ValidateCoupon;

public class ValidateCouponCommandHandler : IRequestHandler<ValidateCouponCommand>
{
    public Task Handle(ValidateCouponCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        return Task.CompletedTask;
    }
}
