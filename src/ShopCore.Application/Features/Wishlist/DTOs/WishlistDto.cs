namespace ShopCore.Application.Wishlist.DTOs;

public record WishlistDto
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public List<WishlistItemDto> Items { get; init; } = new();
    public int TotalItems { get; init; }
}

public record WishlistItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public bool IsInStock { get; init; }
    public DateTime AddedAt { get; init; }
}