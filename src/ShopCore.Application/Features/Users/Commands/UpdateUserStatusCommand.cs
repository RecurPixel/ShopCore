namespace ShopCore.Application.Users.Commands.UpdateUserStatus;

public record UpdateUserStatusCommand(
    int UserId,
    UserStatus Status
) : IRequest;

public enum UserStatus
{
    Active,
    Suspended,
    Deactivated
}
