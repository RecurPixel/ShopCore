using ShopCore.Application.CustomerInvitations.Commands.RejectInvitation;

namespace ShopCore.Application.CustomerInvitations.Validators;

public class RejectInvitationCommandValidator : AbstractValidator<RejectInvitationCommand>
{
    public RejectInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationToken)
            .NotEmpty()
            .WithMessage("Invitation token is required");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Reason))
            .WithMessage("Reason cannot exceed 500 characters");
    }
}
