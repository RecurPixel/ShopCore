using ShopCore.Application.Vendors.Commands.ApproveVendor;

namespace ShopCore.Application.Vendors.Validators;

public class ApproveVendorCommandValidator : AbstractValidator<ApproveVendorCommand>
{
    public ApproveVendorCommandValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("Vendor ID is required");
    }
}
