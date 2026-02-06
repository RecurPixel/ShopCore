using ShopCore.Application.Payouts.Commands.CreatePayout;

namespace ShopCore.Application.Payouts.Validators;

public class CreatePayoutCommandValidator : AbstractValidator<CreatePayoutCommand>
{
    public CreatePayoutCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID is required");

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");

        RuleFor(x => x.Notes)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Notes))
            .WithMessage("Notes cannot exceed 500 characters");
    }
}
