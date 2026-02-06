using ShopCore.Application.CustomerInvitations.Commands.AcceptInvitation;

namespace ShopCore.Application.CustomerInvitations.Validators;

public class AcceptInvitationCommandValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationToken)
            .NotEmpty()
            .WithMessage("Invitation token is required");

        RuleFor(x => x.Password)
            .MinimumLength(8)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage("Password must be at least 8 characters");

        RuleFor(x => x.AgreeToTerms)
            .Equal(true)
            .WithMessage("You must agree to the terms and conditions");
    }
}
