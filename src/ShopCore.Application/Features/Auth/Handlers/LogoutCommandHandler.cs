namespace ShopCore.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public LogoutCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(_currentUser.UserId);

        if (user == null)
            throw new NotFoundException(nameof(User), _currentUser.UserId ?? 0);

        // Invalidate refresh token
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
