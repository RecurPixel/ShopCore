namespace ShopCore.Application.Products.DTOs;

public record ProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public decimal DiscountPercentage { get; init; }
    public bool IsInStock { get; init; }
    public bool IsOnSale { get; init; }
    public string? PrimaryImageUrl { get; init; }
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public string? Status = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
}
