namespace ShopCore.Application.CustomerInvitations.Commands.ResendInvitation;

public record ResendInvitationCommand(int InvitationId) : IRequest<bool>;
