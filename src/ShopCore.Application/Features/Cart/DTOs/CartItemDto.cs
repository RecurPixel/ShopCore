namespace ShopCore.Application.Cart.DTOs;

public record CartItemDto
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public decimal Subtotal { get; init; }
    public bool IsInStock { get; init; }
    public int VendorId { get; init; }
    public string VendorName { get; init; } = string.Empty;
}
