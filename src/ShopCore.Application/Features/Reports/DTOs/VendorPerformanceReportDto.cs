namespace ShopCore.Application.Reports.DTOs;

public record VendorPerformanceReportDto(
    int TotalVendors,
    int ActiveVendors,
    List<TopVendorDto> TopVendors,
    decimal AverageRating
);

public record TopVendorDto(
    int VendorId,
    string VendorName,
    decimal Revenue,
    int OrderCount,
    decimal Rating
);
