namespace ShopCore.Domain.Entities;

public class Wishlist : AuditableEntity
{
    public int UserId { get; set; } // FK
    public int ProductId { get; set; } // FK

    // Navigation
    public required User User { get; set; }

    public required Product Product { get; set; }
}
