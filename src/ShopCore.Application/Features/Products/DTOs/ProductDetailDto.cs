namespace ShopCore.Application.Features.Products.DTOs;

public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public int StockQuantity { get; set; }
    public string? SKU { get; set; }
    public bool IsInStock { get; set; }
    public bool IsOnSale { get; set; }
    public bool IsFeatured { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int ViewCount { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductSpecificationDto> Specifications { get; set; } = new();
}
