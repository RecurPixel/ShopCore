using ShopCore.Application.CustomerInvitations.Commands.CancelInvitation;

namespace ShopCore.Application.CustomerInvitations.Validators;

public class CancelInvitationCommandValidator : AbstractValidator<CancelInvitationCommand>
{
    public CancelInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId)
            .GreaterThan(0)
            .WithMessage("Invitation ID is required");
    }
}
