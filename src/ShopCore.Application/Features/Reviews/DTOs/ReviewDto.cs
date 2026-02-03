namespace ShopCore.Application.Reviews.DTOs;

public record ReviewDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public int UserId { get; init; }
    public string UserName { get; init; } = string.Empty;
    public string? UserAvatar { get; init; }
    public int Rating { get; init; }
    public string? Title { get; init; }
    public string? Comment { get; init; }
    public List<string>? ImageUrls { get; init; }
    public bool IsVerifiedPurchase { get; init; }
    public bool IsApproved { get; init; }
    public int HelpfulCount { get; init; }
    public string? VendorResponse { get; init; }
    public DateTime? VendorRespondedAt { get; init; }
    public DateTime CreatedAt { get; init; }
}
