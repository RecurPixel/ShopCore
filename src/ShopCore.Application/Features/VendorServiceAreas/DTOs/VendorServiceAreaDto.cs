namespace ShopCore.Application.VendorServiceAreas.DTOs;

public record VendorServiceAreaDto(
    int Id,
    int VendorId,
    string AreaName,
    string City,
    string State,
    List<string> Pincodes,
    bool IsActive,
    DateTime CreatedAt
);
