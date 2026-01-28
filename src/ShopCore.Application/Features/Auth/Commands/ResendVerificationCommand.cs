namespace ShopCore.Application.Auth.Commands.ResendVerification;

public record ResendVerificationCommand(string Email) : IRequest;
