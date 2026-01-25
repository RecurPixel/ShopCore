namespace ShopCore.Application.Wishlist.DTOs;

public record WishlistDto(int Id, int UserId, List<WishlistItemDto> Items, int TotalItems);

public record WishlistItemDto(
    int Id,
    int ProductId,
    string ProductName,
    string? ProductImageUrl,
    decimal Price,
    decimal? CompareAtPrice,
    bool IsInStock,
    DateTime AddedAt
);
