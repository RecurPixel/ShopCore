namespace ShopCore.Application.Auth.Commands.VerifyEmail;

public record VerifyEmailCommand(string VerificationToken) : IRequest;
