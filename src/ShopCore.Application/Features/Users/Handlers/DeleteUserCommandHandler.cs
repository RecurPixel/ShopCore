namespace ShopCore.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteUserCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can delete users");

        var user = await _context.Users
            .Include(u => u.Addresses)
            .Include(u => u.Wishlists)
            .FirstOrDefaultAsync(u => u.Id == request.Id, ct);

        if (user == null)
            throw new NotFoundException("User", request.Id);

        // Check for active subscriptions
        var hasActiveSubscriptions = await _context.Subscriptions
            .AnyAsync(s => s.CustomerId == request.Id && s.Status == SubscriptionStatus.Active, ct);

        if (hasActiveSubscriptions)
            throw new BadRequestException("Cannot delete user with active subscriptions");

        // Soft delete - mark as inactive and anonymize
        user.IsActive = false;
        user.Email = $"deleted_{user.Id}@deleted.com";
        user.FirstName = "Deleted";
        user.LastName = "User";
        user.PhoneNumber = string.Empty;
        user.RefreshToken = null;

        // Remove addresses
        foreach (var address in user.Addresses)
        {
            address.IsDeleted = true;
        }

        // Remove wishlists
        _context.Wishlists.RemoveRange(user.Wishlists);

        await _context.SaveChangesAsync(ct);
    }
}
