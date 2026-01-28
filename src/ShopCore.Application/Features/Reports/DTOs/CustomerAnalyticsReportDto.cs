namespace ShopCore.Application.Reports.DTOs;

public record CustomerAnalyticsReportDto(
    int TotalCustomers,
    int NewCustomersThisMonth,
    int ActiveCustomers,
    decimal AverageOrderValue,
    decimal CustomerRetentionRate,
    List<CustomerSegmentDto> CustomerSegments
);

public record CustomerSegmentDto(
    string Segment,
    int CustomerCount,
    decimal TotalSpent
);
