namespace ShopCore.Application.Orders.DTOs;

public record OrderStatusHistoryDto
{
    public string Status { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public DateTime ChangedAt { get; init; }
}
