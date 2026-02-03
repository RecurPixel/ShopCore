namespace ShopCore.Application.Products.DTOs;

public record ProductDetailDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ShortDescription { get; init; }
    public decimal Price { get; init; }
    public decimal? CompareAtPrice { get; init; }
    public decimal DiscountPercentage { get; init; }
    public int StockQuantity { get; init; }
    public string? SKU { get; init; }
    public bool IsInStock { get; init; }
    public bool IsOnSale { get; init; }
    public bool IsFeatured { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal AverageRating { get; init; }
    public int ReviewCount { get; init; }
    public int ViewCount { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public List<ProductImageDto> Images { get; init; } = new();
    public List<ProductSpecificationDto> Specifications { get; init; } = new();
}
