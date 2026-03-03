namespace ShopCore.Application.Notifications.Queries.GetUnreadCount;

public record GetUnreadCountQuery : IRequest<int>;

public class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public Task<int> Handle(GetUnreadCountQuery request, CancellationToken ct)
        => _context.Notifications
            .CountAsync(n => n.UserId == _currentUser.RequiredUserId && !n.IsRead, ct);
}
