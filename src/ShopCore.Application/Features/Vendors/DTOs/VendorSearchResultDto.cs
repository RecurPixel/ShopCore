namespace ShopCore.Application.Vendors.DTOs;

public record VendorSearchResultDto(
    int VendorId,
    string BusinessName,
    string? BusinessDescription,
    string? LogoUrl,
    decimal AverageRating,
    int TotalReviews,
    int TotalProducts,
    List<string> ServingAreas
);
