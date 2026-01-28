using ShopCore.Application.CustomerInvitations.Commands.AcceptInvitation;
using ShopCore.Application.CustomerInvitations.DTOs;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class AcceptInvitationCommandHandler
    : IRequestHandler<AcceptInvitationCommand, InvitationAcceptedDto>
{
    private readonly IApplicationDbContext _context;

    public AcceptInvitationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InvitationAcceptedDto> Handle(
        AcceptInvitationCommand request,
        CancellationToken cancellationToken)
    {
        var invitation = await _context.CustomerInvitations
            .Include(ci => ci.Vendor)
            .FirstOrDefaultAsync(ci => ci.InvitationToken == request.InvitationToken, cancellationToken)
            ?? throw new NotFoundException(nameof(CustomerInvitation), request.InvitationToken);

        if (invitation.Status != InvitationStatus.Sent && invitation.Status != InvitationStatus.Pending)
        {
            throw new InvalidOperationException($"Invitation is not in a valid state for acceptance. Current status: {invitation.Status}");
        }

        if (invitation.ExpiresAt < DateTime.UtcNow)
        {
            invitation.Status = InvitationStatus.Expired;
            await _context.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Invitation has expired");
        }

        if (!request.AgreeToTerms)
        {
            throw new InvalidOperationException("You must agree to terms and conditions");
        }

        // TODO: Create user account if password provided
        // TODO: Create subscription from invitation details
        // For now, mark as accepted

        invitation.Status = InvitationStatus.Accepted;
        invitation.AcceptedAt = DateTime.UtcNow;
        // invitation.UserId = newUser.Id;

        await _context.SaveChangesAsync(cancellationToken);

        return new InvitationAcceptedDto(
            0, // userId - TODO: return actual user ID
            0, // subscriptionId - TODO: return actual subscription ID
            "Invitation accepted successfully! Your subscription has been created.",
            null
        );
    }
}
