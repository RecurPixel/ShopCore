namespace ShopCore.Application.AdminDashboard.DTOs;

public record AdminDashboardStatsDto
{
    public int TotalUsers { get; init; }
    public int TotalVendors { get; init; }
    public int TotalProducts { get; init; }
    public int TotalOrders { get; init; }
    public int TotalSubscriptions { get; init; }
    public int PendingOrders { get; init; }
    public int PendingVendorApprovals { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal RevenueThisMonth { get; init; }
    public decimal RevenueToday { get; init; }
}