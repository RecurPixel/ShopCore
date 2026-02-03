namespace ShopCore.Application.Location.DTOs;

public record NearbyVendorDto
{
    public int VendorId { get; init; }
    public string BusinessName { get; init; } = string.Empty;
    public string? LogoUrl { get; init; }
    public decimal Rating { get; init; }
    public int ReviewCount { get; init; }
    public double Distance { get; init; }
    public bool IsOpen { get; init; }
    public List<string> ServiceAreas { get; init; } = new();
}