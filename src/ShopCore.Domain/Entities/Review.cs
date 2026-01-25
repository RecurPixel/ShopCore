namespace ShopCore.Domain.Entities;

public class Review : AuditableEntity
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int? OrderItemId { get; set; } // Link to purchase for verification

    // Review content
    public int Rating { get; set; } // 1-5 stars
    public string? Title { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? ImageUrls { get; set; } // Comma-separated or JSON array

    // Verification & Moderation
    public bool IsVerifiedPurchase { get; set; }
    public bool IsApproved { get; set; } = true; // Auto-approve by default, moderate later if needed
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; } // FK to User (admin)

    // Engagement
    public int HelpfulCount { get; set; }

    // Vendor Response
    public string? VendorResponse { get; set; }
    public DateTime? VendorRespondedAt { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
    public User User { get; set; } = null!;
    public OrderItem? OrderItem { get; set; }
}
