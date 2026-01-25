namespace ShopCore.Domain.Entities;

public class Product : AuditableEntity
{
    // Core Info
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty; // unique, SEO-friendly URL
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    // Pricing
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; } // Original price for "was/now" display
    public decimal? CostPerItem { get; set; } // For vendor profit calculation

    // Inventory
    public int StockQuantity { get; set; }
    public string? SKU { get; set; } // Stock Keeping Unit (unique)
    public string? Barcode { get; set; } // EAN/UPC barcode
    public bool TrackInventory { get; set; } = true;

    // Physical Properties
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; } // "kg", "g", "lb"
    public string? Dimensions { get; set; } // "10x20x30 cm"

    // Status & Features
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public bool IsFeatured { get; set; }

    // Subscription (PRIVATE feature)
    public bool IsSubscriptionAvailable { get; set; }
    public decimal? SubscriptionDiscount { get; set; } // % discount for subscriptions

    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    // Statistics (denormalized for performance)
    public int ViewCount { get; set; }
    public int OrderCount { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }

    // Foreign Keys
    public int VendorId { get; set; }
    public int CategoryId { get; set; }

    // Computed Properties
    public decimal DiscountPercentage =>
        CompareAtPrice.HasValue && CompareAtPrice > 0
            ? Math.Round((CompareAtPrice.Value - Price) / CompareAtPrice.Value * 100, 2)
            : 0;

    public bool IsInStock => !TrackInventory || StockQuantity > 0;

    public bool IsOnSale => CompareAtPrice.HasValue && CompareAtPrice > Price;

    // Navigation Properties
    public VendorProfile Vendor { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductSpecification> Specifications { get; set; } =
        new List<ProductSpecification>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<SubscriptionItem> SubscriptionItems { get; set; } =
        new List<SubscriptionItem>();
}
