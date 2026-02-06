using ShopCore.Application.Addresses.Commands.DeleteAddress;

namespace ShopCore.Application.Addresses.Validators;

public class DeleteAddressCommandValidator : AbstractValidator<DeleteAddressCommand>
{
    public DeleteAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Address ID is required");
    }
}
