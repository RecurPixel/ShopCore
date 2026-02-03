namespace ShopCore.Application.AdminDashboard.DTOs;

public record RevenueStatsDto
{
    public decimal TotalRevenue { get; init; }
    public decimal RevenueThisMonth { get; init; }
    public decimal RevenueLastMonth { get; init; }
    public decimal RevenueThisWeek { get; init; }
    public decimal RevenueToday { get; init; }
    public decimal GrowthPercentage { get; init; }
    public List<DailyRevenueDto> DailyRevenue { get; init; } = new();
}

public record DailyRevenueDto
{
    public DateTime Date { get; init; }
    public decimal Revenue { get; init; }
    public int OrderCount { get; init; }
}