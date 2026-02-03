namespace ShopCore.Application.Users.DTOs;

public record UserProfileDto
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
    public DateTime? LastLoginAt { get; init; }
}
