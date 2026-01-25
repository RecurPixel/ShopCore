namespace ShopCore.Domain.Entities;

public class CartItem : AuditableEntity
{
    public int CartId { get; set; } // FK
    public int ProductId { get; set; } // FK
    public int Quantity { get; set; }
    public decimal Price { get; set; } // Price snapshot at add time

    // Computed property
    public decimal Subtotal => Quantity * Price;

    // Navigation
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
