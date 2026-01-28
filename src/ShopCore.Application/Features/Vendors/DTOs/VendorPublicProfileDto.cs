namespace ShopCore.Application.Vendors.DTOs;

public record VendorPublicProfileDto(
    int Id,
    string BusinessName,
    string? Description,
    string? LogoUrl,
    string? BannerUrl,
    decimal Rating,
    int ReviewCount,
    List<string> ServiceAreas,
    bool IsOpen,
    string? OpeningHours
);
