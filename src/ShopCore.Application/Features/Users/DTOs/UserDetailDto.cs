namespace ShopCore.Application.Users.DTOs;

public record UserDetailDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
    public int AddressCount { get; init; }
    public int OrderCount { get; init; }
    public int SubscriptionCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}