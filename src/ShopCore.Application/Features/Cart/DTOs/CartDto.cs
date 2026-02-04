namespace ShopCore.Application.Cart.DTOs;

public record CartDto
{
    public int Id { get; init; }
    public List<CartItemDto> Items { get; init; } = new();
    public decimal Subtotal { get; init; }
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
    public int ItemCount { get; init; }
    public string? AppliedCouponCode { get; init; }
}
