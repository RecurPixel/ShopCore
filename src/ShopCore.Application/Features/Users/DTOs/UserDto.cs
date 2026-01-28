namespace ShopCore.Application.Users.DTOs;

public record UserDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string Role,
    string Status,
    DateTime CreatedAt
);
