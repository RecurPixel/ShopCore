namespace ShopCore.Domain.Entities;

public class Cart : AuditableEntity
{
    public int UserId { get; set; } // FK, unique

    // Navigation
    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
