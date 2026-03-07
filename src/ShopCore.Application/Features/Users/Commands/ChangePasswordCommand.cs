namespace ShopCore.Application.Users.Commands.ChangePassword;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword, string ConfirmNewPassword) : IRequest;
