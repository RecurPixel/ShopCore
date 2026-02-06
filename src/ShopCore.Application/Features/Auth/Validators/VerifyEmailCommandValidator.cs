using ShopCore.Application.Auth.Commands.VerifyEmail;

namespace ShopCore.Application.Auth.Validators;

public class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Invalid email format");

        RuleFor(x => x.VerificationToken)
            .NotEmpty()
            .WithMessage("Verification token is required");
    }
}
