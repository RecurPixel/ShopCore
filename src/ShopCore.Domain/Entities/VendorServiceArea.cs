namespace ShopCore.Domain.Entities;

public class VendorServiceArea : AuditableEntity
{
    public int VendorId { get; set; }

    // Area definition
    public string AreaName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public List<string> Pincodes { get; set; } = new();

    // Optional geofencing (Phase 2)
    public string? GeoJsonPolygon { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
}
