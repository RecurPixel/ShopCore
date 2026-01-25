using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResponse>;
