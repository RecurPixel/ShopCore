using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetUserByIdQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<UserDetailDto?> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        if (_currentUser.Role != UserRole.Admin)
            throw new ForbiddenException("Only admins can view user details");

        var user = await _context.Users
            .Where(u => u.Id == request.Id)
            .Select(u => new UserDetailDto(
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.PhoneNumber,
                u.AvatarUrl,
                u.Role.ToString(),
                u.IsActive ? "Active" : "Inactive",
                u.IsEmailVerified,
                u.Addresses.Count,
                u.Orders.Count,
                u.Subscriptions.Count,
                u.CreatedAt,
                u.LastLoginAt
            ))
            .FirstOrDefaultAsync(ct);

        return user;
    }
}
