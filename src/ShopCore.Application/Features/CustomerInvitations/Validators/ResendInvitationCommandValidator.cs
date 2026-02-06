using ShopCore.Application.CustomerInvitations.Commands.ResendInvitation;

namespace ShopCore.Application.CustomerInvitations.Validators;

public class ResendInvitationCommandValidator : AbstractValidator<ResendInvitationCommand>
{
    public ResendInvitationCommandValidator()
    {
        RuleFor(x => x.InvitationId)
            .GreaterThan(0)
            .WithMessage("Invitation ID is required");
    }
}
