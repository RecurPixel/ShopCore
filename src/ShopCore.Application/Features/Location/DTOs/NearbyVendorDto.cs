namespace ShopCore.Application.Location.DTOs;

public record NearbyVendorDto(
    int VendorId,
    string BusinessName,
    string? LogoUrl,
    decimal Rating,
    int ReviewCount,
    double Distance,
    bool IsOpen,
    List<string> ServiceAreas
);
