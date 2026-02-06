using ShopCore.Application.Auth.Commands.ResendVerification;

namespace ShopCore.Application.Auth.Validators;

public class ResendVerificationCommandValidator : AbstractValidator<ResendVerificationCommand>
{
    public ResendVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");
    }
}
