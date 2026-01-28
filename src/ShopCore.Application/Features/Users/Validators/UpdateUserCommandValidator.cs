using ShopCore.Application.Users.Commands.UpdateCurrentUser;

namespace ShopCore.Application.Users.Validators;

public class UpdateCurrentUserCommandValidator : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50);

        RuleFor(x => x.LastName).NotEmpty().WithMessage("Last name is required").MaximumLength(50);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .Matches(@"^\d{10}$")
            .WithMessage("Phone number must be 10 digits");
    }
}
