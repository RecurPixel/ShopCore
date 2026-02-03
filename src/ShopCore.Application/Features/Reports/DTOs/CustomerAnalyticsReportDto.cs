namespace ShopCore.Application.Reports.DTOs;

public record CustomerAnalyticsReportDto
{
    public int TotalCustomers { get; init; }
    public int NewCustomersThisMonth { get; init; }
    public int ActiveCustomers { get; init; }
    public decimal AverageOrderValue { get; init; }
    public decimal CustomerRetentionRate { get; init; }
    public List<CustomerSegmentDto> CustomerSegments { get; init; } = new();
}

public record CustomerSegmentDto
{
    public string Segment { get; init; } = string.Empty;
    public int CustomerCount { get; init; }
    public decimal TotalSpent { get; init; }
}