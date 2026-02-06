using ShopCore.Application.VendorServiceAreas.Commands.RemoveVendorServiceArea;

namespace ShopCore.Application.VendorServiceAreas.Validators;

public class RemoveVendorServiceAreaCommandValidator : AbstractValidator<RemoveVendorServiceAreaCommand>
{
    public RemoveVendorServiceAreaCommandValidator()
    {
        RuleFor(x => x.ServiceAreaId)
            .GreaterThan(0)
            .WithMessage("Service area ID is required");
    }
}
