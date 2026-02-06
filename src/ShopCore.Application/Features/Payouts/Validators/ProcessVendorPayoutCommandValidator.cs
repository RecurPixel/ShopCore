using ShopCore.Application.Payouts.Commands.ProcessVendorPayout;

namespace ShopCore.Application.Payouts.Validators;

public class ProcessVendorPayoutCommandValidator : AbstractValidator<ProcessVendorPayoutCommand>
{
    public ProcessVendorPayoutCommandValidator()
    {
        RuleFor(x => x.PayoutId)
            .GreaterThan(0)
            .WithMessage("Payout ID is required");

        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .WithMessage("Transaction ID is required")
            .MaximumLength(100)
            .WithMessage("Transaction ID cannot exceed 100 characters");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");
    }
}
