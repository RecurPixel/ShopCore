namespace ShopCore.Domain.Entities;

public class ProductSpecification
{
    public int Id { get; set; }
    public int ProductId { get; set; } // FK
    public string Name { get; set; } = string.Empty; // "Weight", "Color"
    public string Value { get; set; } = string.Empty; // "1kg", "Red"
    public int DisplayOrder { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
}
