using ShopCore.Application.Payouts.Commands.CancelPayout;

namespace ShopCore.Application.Payouts.Validators;

public class CancelPayoutCommandValidator : AbstractValidator<CancelPayoutCommand>
{
    public CancelPayoutCommandValidator()
    {
        RuleFor(x => x.PayoutId)
            .GreaterThan(0)
            .WithMessage("Payout ID is required");
    }
}
