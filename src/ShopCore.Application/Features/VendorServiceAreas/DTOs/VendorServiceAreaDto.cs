namespace ShopCore.Application.VendorServiceAreas.DTOs;

public record VendorServiceAreaDto
{
    public int Id { get; init; }
    public int VendorId { get; init; }
    public string AreaName { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public List<string> Pincodes { get; init; } = new();
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
}