namespace ShopCore.Application.CustomerInvitations.Commands.RejectInvitation;

public record RejectInvitationCommand(
    string InvitationToken,
    string? Reason
) : IRequest<bool>;
