using ShopCore.Application.Payouts.Commands.ProcessPayout;

namespace ShopCore.Application.Payouts.Validators;

public class ProcessPayoutCommandValidator : AbstractValidator<ProcessPayoutCommand>
{
    public ProcessPayoutCommandValidator()
    {
        RuleFor(x => x.PayoutId)
            .GreaterThan(0)
            .WithMessage("Payout ID is required");
    }
}
