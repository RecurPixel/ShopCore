namespace ShopCore.Application.Users.Commands.UpdateUserStatus;

public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateUserStatusCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(UpdateUserStatusCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can update user status");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user == null)
            throw new NotFoundException("User", request.UserId);

        user.IsActive = request.Status switch
        {
            UserStatus.Active => true,
            UserStatus.Suspended => false,
            UserStatus.Deactivated => false,
            _ => user.IsActive
        };

        await _context.SaveChangesAsync(ct);
    }
}
