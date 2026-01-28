using System.Text.RegularExpressions;
using ShopCore.Application.VendorServiceAreas.Commands.UpdateVendorServiceArea;

namespace ShopCore.Application.VendorServiceAreas.Validators;

public class UpdateVendorServiceAreaCommandValidator : AbstractValidator<UpdateVendorServiceAreaCommand>
{
    public UpdateVendorServiceAreaCommandValidator()
    {
        RuleFor(x => x.ServiceAreaId)
            .GreaterThan(0)
            .WithMessage("Invalid service area ID");

        RuleFor(x => x.AreaName)
            .NotEmpty()
            .WithMessage("Area name is required")
            .MaximumLength(100);

        RuleFor(x => x.Pincodes)
            .NotEmpty()
            .WithMessage("At least one pincode is required")
            .Must(pincodes => pincodes.All(p => Regex.IsMatch(p, @"^\d{6}$")))
            .WithMessage("All pincodes must be 6 digits");
    }
}
