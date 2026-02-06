using ShopCore.Application.Vendors.Commands.SuspendVendor;

namespace ShopCore.Application.Vendors.Validators;

public class SuspendVendorCommandValidator : AbstractValidator<SuspendVendorCommand>
{
    public SuspendVendorCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID is required");

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage("Reason is required")
            .MaximumLength(500)
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
