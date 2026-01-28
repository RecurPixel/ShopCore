using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Commands.AcceptInvitation;

public record AcceptInvitationCommand(
    string InvitationToken,
    string? Password,
    bool AgreeToTerms
) : IRequest<InvitationAcceptedDto>;
