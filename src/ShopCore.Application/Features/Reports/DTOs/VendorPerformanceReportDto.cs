namespace ShopCore.Application.Reports.DTOs;

public record VendorPerformanceReportDto
{
    public int TotalVendors { get; init; }
    public int ActiveVendors { get; init; }
    public List<TopVendorDto> TopVendors { get; init; } = new();
    public decimal AverageRating { get; init; }
}

public record TopVendorDto
{
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
    public int OrderCount { get; init; }
    public decimal Rating { get; init; }
}
