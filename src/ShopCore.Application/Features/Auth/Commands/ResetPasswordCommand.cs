namespace ShopCore.Application.Auth.Commands.ForgotPassword;

public record ResetPasswordCommand(string Email, string ResetToken, string NewPassword)
    : IRequest<Unit>;
