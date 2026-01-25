namespace ShopCore.Application.AdminDashboard.DTOs;

public record RevenueStatsDto(
    decimal TotalRevenue,
    decimal RevenueThisMonth,
    decimal RevenueLastMonth,
    decimal RevenueThisWeek,
    decimal RevenueToday,
    decimal GrowthPercentage,
    List<DailyRevenueDto> DailyRevenue
);

public record DailyRevenueDto(DateTime Date, decimal Revenue, int OrderCount);
