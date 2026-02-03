namespace ShopCore.Application.Reports.DTOs;

public record RevenueReportDto
{
    public decimal TotalRevenue { get; init; }
    public decimal OrderRevenue { get; init; }
    public decimal SubscriptionRevenue { get; init; }
    public decimal GrowthPercentage { get; init; }
    public List<RevenueByPeriodDto> RevenueByPeriod { get; init; } = new();
}

public record RevenueByPeriodDto
{
    public string Period { get; init; } = string.Empty;
    public decimal Revenue { get; init; }
}