namespace ShopCore.Domain.Entities;

public class User : AuditableEntity
{
    // Basic Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    // Verification (consistent "Is" prefix)
    public bool IsEmailVerified { get; set; }
    public bool IsPhoneVerified { get; set; }
    public string? EmailVerificationToken { get; set; }
    public DateTime? EmailVerificationTokenExpiry { get; set; }

    // Password Reset
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }

    // Authentication
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Profile & Activity
    public string? AvatarUrl { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Computed property
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    public VendorProfile? VendorProfile { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    public ICollection<CustomerInvitation> ReceivedInvitations { get; set; } = new List<CustomerInvitation>();
}
