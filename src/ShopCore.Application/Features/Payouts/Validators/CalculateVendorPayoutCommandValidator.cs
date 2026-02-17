using ShopCore.Application.Payouts.Commands.CalculateVendorPayout;

namespace ShopCore.Application.Payouts.Validators;

public class CalculateVendorPayoutCommandValidator : AbstractValidator<CalculateVendorPayoutCommand>
{
    public CalculateVendorPayoutCommandValidator()
    {
        // VendorId is optional - if not provided, handler uses auth context
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .When(x => x.VendorId.HasValue)
            .WithMessage("Vendor ID must be greater than 0 if provided");

        RuleFor(x => x.FromDate)
            .LessThanOrEqualTo(x => x.ToDate)
            .WithMessage("From date must be before or equal to To date");

        RuleFor(x => x.ToDate)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("To date cannot be in the future");
    }
}
