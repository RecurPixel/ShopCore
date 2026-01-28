using System.Text.RegularExpressions;
using ShopCore.Application.VendorServiceAreas.Commands.AddVendorServiceArea;

namespace ShopCore.Application.VendorServiceAreas.Validators;

public class AddVendorServiceAreaCommandValidator : AbstractValidator<AddVendorServiceAreaCommand>
{
    public AddVendorServiceAreaCommandValidator()
    {
        RuleFor(x => x.AreaName)
            .NotEmpty()
            .WithMessage("Area name is required")
            .MaximumLength(100);

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City is required")
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .WithMessage("State is required")
            .MaximumLength(100);

        RuleFor(x => x.Pincodes)
            .NotEmpty()
            .WithMessage("At least one pincode is required")
            .Must(pincodes => pincodes.All(p => Regex.IsMatch(p, @"^\d{6}$")))
            .WithMessage("All pincodes must be 6 digits");
    }
}
