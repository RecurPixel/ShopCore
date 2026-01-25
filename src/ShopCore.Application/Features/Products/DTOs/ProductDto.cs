namespace ShopCore.Application.Features.Products.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsInStock { get; set; }
    public bool IsOnSale { get; set; }
    public string? PrimaryImageUrl { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string VendorName { get; set; } = string.Empty;
}
