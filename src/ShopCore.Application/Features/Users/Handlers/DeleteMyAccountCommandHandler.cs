using ShopCore.Application.Users.Commands.DeleteMyAccount;

namespace ShopCore.Application.Users.Commands.DeleteMyAccount;

public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteMyAccountCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == _currentUser.UserId, cancellationToken);

        if (user is null)
            return;

        // Soft delete
        user.IsDeleted = true;
        user.DeletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
    }
}
