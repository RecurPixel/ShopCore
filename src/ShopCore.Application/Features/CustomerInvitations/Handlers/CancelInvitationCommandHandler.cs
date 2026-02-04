using ShopCore.Application.CustomerInvitations.Commands.CancelInvitation;

namespace ShopCore.Application.CustomerInvitations.Handlers;

public class CancelInvitationCommandHandler
    : IRequestHandler<CancelInvitationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public CancelInvitationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(
        CancelInvitationCommand request,
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
            throw new InvalidOperationException("Cannot cancel an already accepted invitation");
        }

        invitation.IsDeleted = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
