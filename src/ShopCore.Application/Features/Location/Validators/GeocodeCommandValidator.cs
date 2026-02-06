using ShopCore.Application.Location.Commands.Geocode;

namespace ShopCore.Application.Location.Validators;

public class GeocodeCommandValidator : AbstractValidator<GeocodeCommand>
{
    public GeocodeCommandValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MaximumLength(500)
            .WithMessage("Address cannot exceed 500 characters");
    }
}
