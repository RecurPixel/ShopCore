using ShopCore.Application.CustomerInvitations.Commands.ResendInvitation;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class ResendInvitationCommandHandler
    : IRequestHandler<ResendInvitationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public ResendInvitationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        ResendInvitationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var vendor = await _context.VendorProfiles
            .FirstOrDefaultAsync(v => v.UserId == userId, cancellationToken)
            ?? throw new NotFoundException(nameof(VendorProfile), userId);

        var invitation = await _context.CustomerInvitations
            .FirstOrDefaultAsync(ci => ci.Id == request.InvitationId && ci.VendorId == vendor.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(CustomerInvitation), request.InvitationId);

        if (invitation.Status == InvitationStatus.Accepted)
        {
            throw new InvalidOperationException("Cannot resend an already accepted invitation");
        }

        // Extend expiry
        invitation.ExpiresAt = DateTime.UtcNow.AddDays(7);
        invitation.Status = InvitationStatus.Sent;

        // TODO: Actually resend via SMS/WhatsApp/Email

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
