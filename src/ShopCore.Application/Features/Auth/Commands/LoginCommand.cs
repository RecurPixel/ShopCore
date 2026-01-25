using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResponse>;
