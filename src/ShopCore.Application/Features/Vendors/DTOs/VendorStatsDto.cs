namespace ShopCore.Application.Vendors.DTOs;

public record VendorStatsDto(
    int TotalProducts,
    int ActiveProducts,
    int TotalOrders,
    int PendingOrders,
    int CompletedOrders,
    int TotalSubscriptions,
    int ActiveSubscriptions,
    decimal TotalRevenue,
    decimal RevenueThisMonth,
    decimal PendingPayout,
    decimal Rating,
    int TotalReviews
);
