namespace ShopCore.Application.Reports.DTOs;

public record RevenueReportDto(
    decimal TotalRevenue,
    decimal OrderRevenue,
    decimal SubscriptionRevenue,
    decimal GrowthPercentage,
    List<RevenueByPeriodDto> RevenueByPeriod
);

public record RevenueByPeriodDto(
    string Period,
    decimal Revenue
);
