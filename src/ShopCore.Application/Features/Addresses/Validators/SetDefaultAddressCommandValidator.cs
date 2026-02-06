using ShopCore.Application.Addresses.Commands.SetDefaultAddress;

namespace ShopCore.Application.Addresses.Validators;

public class SetDefaultAddressCommandValidator : AbstractValidator<SetDefaultAddressCommand>
{
    public SetDefaultAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Address ID is required");
    }
}
