using ShopCore.Application.Users.DTOs;

namespace ShopCore.Application.Users.Commands.UpdateCurrentUser;

public record UpdateCurrentUserCommand(string FirstName, string LastName, string PhoneNumber)
    : IRequest<UserProfileDto>;
