namespace ShopCore.Application.Vendors.DTOs;

public record VendorPublicProfileDto
{
    public int Id { get; init; }
    public string BusinessName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? LogoUrl { get; init; }
    public string? BannerUrl { get; init; }
    public decimal Rating { get; init; }
    public int ReviewCount { get; init; }
    public List<string> ServiceAreas { get; init; } = new();
    public bool IsOpen { get; init; }
    public string? OpeningHours { get; init; }
}