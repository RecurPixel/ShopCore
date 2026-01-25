namespace ShopCore.Application.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string Email, string VerificationToken) : IRequest;
