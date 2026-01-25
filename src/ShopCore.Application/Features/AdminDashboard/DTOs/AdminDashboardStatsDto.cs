namespace ShopCore.Application.AdminDashboard.DTOs;

public record AdminDashboardStatsDto(
    int TotalUsers,
    int TotalVendors,
    int TotalProducts,
    int TotalOrders,
    int TotalSubscriptions,
    int PendingOrders,
    int PendingVendorApprovals,
    decimal TotalRevenue,
    decimal RevenueThisMonth,
    decimal RevenueToday
);
