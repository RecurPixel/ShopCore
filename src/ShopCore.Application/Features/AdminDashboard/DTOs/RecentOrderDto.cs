namespace ShopCore.Application.AdminDashboard.DTOs;

public record RecentOrderDto
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string? CustomerEmail { get; init; }
    public decimal Total { get; init; }
    public OrderStatus Status { get; init; }
    public DateTime OrderDate { get; init; }
}