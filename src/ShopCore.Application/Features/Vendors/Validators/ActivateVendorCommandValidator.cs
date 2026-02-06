using ShopCore.Application.Vendors.Commands.ActivateVendor;

namespace ShopCore.Application.Vendors.Validators;

public class ActivateVendorCommandValidator : AbstractValidator<ActivateVendorCommand>
{
    public ActivateVendorCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID is required");
    }
}
