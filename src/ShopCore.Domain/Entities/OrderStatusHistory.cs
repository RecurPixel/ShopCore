namespace ShopCore.Domain.Entities;

public class OrderStatusHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
    public int? ChangedBy { get; set; }
    public DateTime ChangedAt { get; set; }

    // Navigation
    public Order Order { get; set; } = null!;
}
