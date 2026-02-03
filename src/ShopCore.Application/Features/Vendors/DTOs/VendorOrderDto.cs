namespace ShopCore.Application.Vendors.DTOs;

public record VendorOrderDto
{
    public int OrderId { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string? CustomerPhone { get; init; }
    public string Status { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public decimal SubTotal { get; init; }
    public decimal Total { get; init; }
    public decimal VendorTotal { get; init; }
    public decimal VendorCommission { get; init; }
    public decimal VendorPayout { get; init; }
    public DateTime OrderDate { get; init; }
    public string? CustomerNotes { get; init; }
    public string? AdminNotes { get; init; }
    public int ItemCount { get; init; }
    public List<VendorOrderItemDto> Items { get; init; } = new();

    public DateTime CreatedAt { get; init; }
}
