namespace ShopCore.Application.Notifications.Commands.DeleteNotification;

public record DeleteNotificationCommand(int NotificationId) : IRequest;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteNotificationCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteNotificationCommand request, CancellationToken ct)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(
                n => n.Id == request.NotificationId && n.UserId == _currentUser.RequiredUserId, ct);

        if (notification == null)
            throw new NotFoundException("Notification", request.NotificationId);

        notification.IsDeleted = true;
        await _context.SaveChangesAsync(ct);
    }
}
