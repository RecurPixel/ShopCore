using ShopCore.Application.Auth.DTOs;

namespace ShopCore.Application.Auth.Commands.RegisterUser;

// Note: Role is determined by the system (defaults to Customer, upgraded to Vendor when vendor profile is created)
// Note: IsEmailVerified is determined by the email verification process, not user input
public record RegisterUserCommand(string Email, string Password, string FirstName, string LastName, string PhoneNumber)
    : IRequest<RegisterResponse>;
