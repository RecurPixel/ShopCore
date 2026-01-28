namespace ShopCore.Application.Users.DTOs;

public record UserDetailDto(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? AvatarUrl,
    string Role,
    string Status,
    bool EmailVerified,
    int AddressCount,
    int OrderCount,
    int SubscriptionCount,
    DateTime CreatedAt,
    DateTime? LastLoginAt
);
