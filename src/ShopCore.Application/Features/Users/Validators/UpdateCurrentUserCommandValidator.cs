using ShopCore.Application.Users.Commands.UpdateCurrentUser;

namespace ShopCore.Application.Users.Validators;

public class UpdateCurrentUserCommandValidator : AbstractValidator<UpdateCurrentUserCommand>
{
    public UpdateCurrentUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(50)
            .WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(50)
            .WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number is required")
            .MaximumLength(20)
            .WithMessage("Phone number cannot exceed 20 characters");
    }
}
