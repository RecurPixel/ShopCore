namespace ShopCore.Domain.Entities;

public class CustomerInvitation : AuditableEntity
{
    public int VendorId { get; set; }
    public string InvitationToken { get; set; } = string.Empty;

    // Customer details
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;

    // Subscription details (pre-filled by vendor)
    public string SubscriptionItemsJson { get; set; } = string.Empty;
    public SubscriptionFrequency Frequency { get; set; }
    public string PreferredDeliveryTime { get; set; } = string.Empty;
    public decimal? DepositAmount { get; set; }

    // Status
    public InvitationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public int? InvitedUserId { get; set; }

    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
    public User? InvitedUser { get; set; }
}

/// <summary>
/// Helper class for JSON serialization of subscription items in invitations
/// </summary>
public class InvitationSubscriptionItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
