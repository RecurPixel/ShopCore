using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
{
    public Task<UserProfileDto> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement query logic
        // 1. Get current user from context/claims
        // 2. Fetch user from database with all details
        // 3. Get user's addresses, preferences, etc.
        // 4. Map to UserProfileDto
        // 5. Return user profile
        return Task.FromResult(new UserProfileDto());
    }
}
