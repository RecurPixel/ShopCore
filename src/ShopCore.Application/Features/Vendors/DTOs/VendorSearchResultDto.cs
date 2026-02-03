namespace ShopCore.Application.Vendors.DTOs;

public record VendorSearchResultDto
{
    public int VendorId { get; init; }
    public string BusinessName { get; init; } = string.Empty;
    public string? BusinessDescription { get; init; }
    public string? LogoUrl { get; init; }
    public decimal AverageRating { get; init; }
    public int TotalReviews { get; init; }
    public int TotalProducts { get; init; }
    public List<string> ServingAreas { get; init; } = new();
}