namespace ShopCore.Application.Vendors.DTOs;

public record VendorOrderItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductSKU { get; init; }
    public string? ProductImage { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
    public decimal CommissionRate { get; init; }
    public decimal CommissionAmount { get; init; }
    public decimal VendorAmount { get; init; }
    public string Status { get; init; } = string.Empty;
}
