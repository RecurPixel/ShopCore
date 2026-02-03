namespace ShopCore.Application.Vendors.DTOs;

public record VendorStatsDto
{
    public int TotalProducts { get; init; }
    public int ActiveProducts { get; init; }
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int CompletedOrders { get; init; }
    public int TotalSubscriptions { get; init; }
    public int ActiveSubscriptions { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal RevenueThisMonth { get; init; }
    public decimal PendingPayout { get; init; }
    public decimal Rating { get; init; }
    public int TotalReviews { get; init; }
}