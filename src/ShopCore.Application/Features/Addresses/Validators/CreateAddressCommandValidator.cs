using ShopCore.Application.Addresses.Commands.CreateAddress;

namespace ShopCore.Application.Addresses.Validators;

public class CreateAddressCommandValidator : AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Full name is required").MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number must be 10 digits");

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .WithMessage("Address line 1 is required")
            .MaximumLength(200);

        RuleFor(x => x.City).NotEmpty().WithMessage("City is required").MaximumLength(100);

        RuleFor(x => x.State).NotEmpty().WithMessage("State is required").MaximumLength(100);

        RuleFor(x => x.Pincode)
            .NotEmpty()
            .WithMessage("Pincode is required")
            .Matches(@"^\d{6}$")
            .WithMessage("Pincode must be 6 digits");
    }
}
