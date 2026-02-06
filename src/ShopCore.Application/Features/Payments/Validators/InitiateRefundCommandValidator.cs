using ShopCore.Application.Payments.Commands.InitiateRefund;

namespace ShopCore.Application.Payments.Validators;

public class InitiateRefundCommandValidator : AbstractValidator<InitiateRefundCommand>
{
    public InitiateRefundCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .GreaterThan(0)
            .WithMessage("Order ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Refund amount must be greater than 0");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
