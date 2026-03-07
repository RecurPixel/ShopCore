using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken) : IRequest<RefreshTokenResponse>;
