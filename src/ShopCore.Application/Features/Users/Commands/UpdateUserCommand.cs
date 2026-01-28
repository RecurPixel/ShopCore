using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    int Id,
    string? FirstName,
    string? LastName,
    string? Email,
    string? PhoneNumber
) : IRequest<UserDto>;
