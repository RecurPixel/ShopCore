using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
    public Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement command logic
        // 1. Get user by id from database
        // 2. Update user properties (email, name, etc.)
        // 3. Save changes to database
        // 4. Map and return updated UserDto
        return Task.FromResult(default(UserDto));
    }
}
