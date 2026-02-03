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
            .Select(u => new UserDetailDto
            {
                Id = u.Id,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PhoneNumber = u.PhoneNumber,
                AvatarUrl = u.AvatarUrl,
                Role = u.Role.ToString(),
                Status = u.IsActive ? "Active" : "Inactive",
                IsEmailVerified = u.IsEmailVerified,
                AddressCount = u.Addresses.Count,
                OrderCount = u.Orders.Count,
                SubscriptionCount = u.Subscriptions.Count,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt
            })
            .FirstOrDefaultAsync(ct);

        return user;
    }
}
