using ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

namespace ShopCore.Application.Payouts.Validators;

public class CalculateVendorPayoutCommandValidator : AbstractValidator<CalculateVendorPayoutCommand>
{
    public CalculateVendorPayoutCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID is required");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage("From date must be before or equal to To date");

        RuleFor(x => x.ToDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("To date cannot be in the future");
    }
}
