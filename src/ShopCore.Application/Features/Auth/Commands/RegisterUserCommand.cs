using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RegisterUser;

public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName)
    : IRequest<RegisterResponse>;
