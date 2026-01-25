namespace ShopCore.Application.AdminDashboard.DTOs;

public record RecentOrderDto(
    int Id,
    string OrderNumber,
    string CustomerName,
    string? CustomerEmail,
    decimal Total,
    OrderStatus Status,
    DateTime OrderDate
);
