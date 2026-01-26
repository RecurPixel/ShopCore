using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Commands.UpdateCurrentUser;

public class UpdateCurrentUserCommandHandler
    : IRequestHandler<UpdateCurrentUserCommand, UserProfileDto>
{
    public Task<UserProfileDto> Handle(
        UpdateCurrentUserCommand request,
        CancellationToken cancellationToken
    )
    {
        // TODO: Implement command logic
        return Task.FromResult(new UserProfileDto());
    }
}
