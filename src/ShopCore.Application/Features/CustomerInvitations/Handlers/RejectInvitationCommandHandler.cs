using ShopCore.Application.CustomerInvitations.Commands.RejectInvitation;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class RejectInvitationCommandHandler
    : IRequestHandler<RejectInvitationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public RejectInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(
        RejectInvitationCommand request,
        CancellationToken cancellationToken)
    {
        var invitation = await _context.CustomerInvitations
            .FirstOrDefaultAsync(ci => ci.InvitationToken == request.InvitationToken, cancellationToken)
            ?? throw new NotFoundException(nameof(CustomerInvitation), request.InvitationToken);

        if (invitation.Status == InvitationStatus.Accepted)
        {
            throw new InvalidOperationException("Cannot reject an already accepted invitation");
        }

        invitation.Status = InvitationStatus.Rejected;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
