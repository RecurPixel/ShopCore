namespace ShopCore.Application.Reviews.DTOs;

public record ReviewDto(
    int Id,
    int ProductId,
    string ProductName,
    int UserId,
    string UserName,
    string? UserAvatarUrl,
    int Rating,
    string? Title,
    string? Comment,
    List<string>? ImageUrls,
    bool IsVerifiedPurchase,
    int HelpfulCount,
    string? VendorResponse,
    DateTime? VendorRespondedAt,
    DateTime CreatedAt
);
