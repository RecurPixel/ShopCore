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
        return Task.FromResult(new UserProfileDto());
    }
}
