namespace ShopCore.Application.CustomerInvitations.Commands.CancelInvitation;

public record CancelInvitationCommand(int InvitationId) : IRequest<bool>;
