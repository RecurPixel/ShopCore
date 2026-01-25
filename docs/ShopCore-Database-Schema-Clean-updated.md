# ShopCore - Database Schema (Updated)

**Total Tables:** 21  
**Public Tables:** 16 (E-Commerce)  
**Private Tables:** 5 (Subscriptions with Multi-Item Support)

---

## Entity Relationships

```
User (1) ──────┬──── (1) VendorProfile
               │
               ├──── (N) Address
               ├──── (1) Cart ──── (N) CartItem
               ├──── (N) Order ──── (N) OrderItem ──── (N) OrderStatusHistory
               ├──── (N) Review
               ├──── (N) Wishlist
               └──── (N) Subscription ──── (N) SubscriptionItem
                                      ├──── (N) Delivery ──── (N) DeliveryItem
                                      └──── (N) SubscriptionInvoice

Category (1) ──── (N) Product ──── (N) ProductImage
                                │
                                ├──── (N) ProductSpecification
                                ├──── (N) CartItem
                                ├──── (N) OrderItem
                                ├──── (N) Review
                                ├──── (N) Wishlist
                                └──── (N) SubscriptionItem

VendorProfile (1) ──── (N) Product
                  ├──── (N) VendorPayout
                  └──── (N) Subscription

Coupon (1) ──── (N) Order
```

---

## Enums

```csharp
public enum UserRole 
{ 
    Customer = 1, 
    Vendor = 2, 
    Admin = 3 
}

public enum VendorStatus 
{ 
    PendingApproval = 1, 
    Active = 2, 
    Suspended = 3 
}

public enum OrderStatus 
{ 
    Pending = 1, 
    Confirmed = 2, 
    Processing = 3, 
    Shipped = 4, 
    Delivered = 5, 
    Cancelled = 6, 
    Refunded = 7 
}

public enum PaymentStatus 
{ 
    Unpaid = 1, 
    Pending = 2, 
    Paid = 3, 
    Failed = 4, 
    Refunded = 5 
}

public enum PaymentMethod 
{ 
    Online = 1, 
    CashOnDelivery = 2 
}

public enum ProductStatus 
{ 
    Draft = 1,
    Active = 2, 
    OutOfStock = 3, 
    Discontinued = 4 
}

public enum SubscriptionStatus 
{ 
    Active = 1, 
    Paused = 2, 
    Suspended = 3, 
    Cancelled = 4, 
    Settled = 5 
}

public enum SubscriptionFrequency 
{ 
    Daily = 1, 
    EveryTwoDays = 2, 
    Weekly = 3, 
    BiWeekly = 4, 
    Monthly = 5, 
    Custom = 6 
}

public enum DeliveryStatus 
{ 
    Scheduled = 1, 
    OutForDelivery = 2, 
    Delivered = 3, 
    Failed = 4, 
    Skipped = 5, 
    Cancelled = 6 
}

public enum DeliveryItemStatus
{
    Scheduled = 1,
    Delivered = 2,
    OutOfStock = 3,
    Damaged = 4,
    Skipped = 5
}

public enum InvoiceStatus 
{ 
    Generated = 1, 
    Paid = 2, 
    Overdue = 3, 
    Cancelled = 4, 
    Refunded = 5 
}

public enum PayoutStatus
{
    Pending = 1,
    Approved = 2,
    Processing = 3,
    Paid = 4,
    Failed = 5,
    Cancelled = 6
}

public enum CouponType 
{ 
    Percentage = 1, 
    FixedAmount = 2, 
    FreeShipping = 3 
}
```

---

## Core Entities

### User
```csharp
public class User : AuditableEntity
{
    public int Id { get; set; }
    
    // Basic Info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;          // unique
    public string PhoneNumber { get; set; } = string.Empty;    // unique
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    
    // Verification
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
    
    // Navigation
    public VendorProfile? VendorProfile { get; set; }
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public Cart? Cart { get; set; }
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
```

### VendorProfile
```csharp
public class VendorProfile : AuditableEntity
{
    public int Id { get; set; }
    
    // User Link
    public int UserId { get; set; }                    // FK, unique
    
    // Business Information
    public string BusinessName { get; set; } = string.Empty;
    public string? BusinessDescription { get; set; }
    public string? BusinessLogo { get; set; }
    public string BusinessAddress { get; set; } = string.Empty;
    
    // Legal & Banking
    public string GstNumber { get; set; } = string.Empty;      // unique
    public string PanNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankIfscCode { get; set; } = string.Empty;
    public string BankAccountHolderName { get; set; } = string.Empty;
    
    // Status & Commission
    public VendorStatus Status { get; set; } = VendorStatus.PendingApproval;
    public decimal CommissionRate { get; set; } = 5.00m;       // percentage
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }
    
    // Subscription Settings (PRIVATE)
    public bool RequiresDeposit { get; set; } = false;
    public decimal? DefaultDepositAmount { get; set; }
    public int? DefaultBillingCycleDays { get; set; }
    
    // Statistics (denormalized)
    public decimal AverageRating { get; set; } = 0;
    public int TotalReviews { get; set; } = 0;
    public int TotalProducts { get; set; } = 0;
    public int TotalOrders { get; set; } = 0;
    public decimal TotalRevenue { get; set; } = 0;
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<VendorPayout> Payouts { get; set; } = new List<VendorPayout>();
}
```

### Address
```csharp
public class Address : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }                // FK
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
```

---

## Product Catalog

### Category
```csharp
public class Category : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;           // unique
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }     // FK (self-reference)
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
```

### Product
```csharp
public class Product : AuditableEntity
{
    public int Id { get; set; }
    
    // Core Info
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;  // unique
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    
    // Pricing
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPerItem { get; set; }
    
    // Inventory
    public int StockQuantity { get; set; }
    public string? SKU { get; set; }                  // unique, nullable
    public string? Barcode { get; set; }
    public bool TrackInventory { get; set; } = true;
    
    // Physical Properties
    public decimal? Weight { get; set; }
    public string? WeightUnit { get; set; }           // "kg", "g", "lb"
    public string? Dimensions { get; set; }           // "10x20x30 cm"
    
    // Status & Features
    public ProductStatus Status { get; set; } = ProductStatus.Draft;
    public bool IsFeatured { get; set; }
    
    // Subscription (PRIVATE)
    public bool IsSubscriptionAvailable { get; set; }
    public decimal? SubscriptionDiscount { get; set; }
    
    // SEO
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
    
    // Statistics (denormalized)
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
    
    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductSpecification> Specifications { get; set; } = new List<ProductSpecification>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    public ICollection<SubscriptionItem> SubscriptionItems { get; set; } = new List<SubscriptionItem>();
}
```

### ProductImage
```csharp
public class ProductImage : AuditableEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }             // FK
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int DisplayOrder { get; set; }
    
    // Navigation
    public Product Product { get; set; } = null!;
}
```

### ProductSpecification
```csharp
public class ProductSpecification
{
    public int Id { get; set; }
    public int ProductId { get; set; }             // FK
    public string Name { get; set; } = string.Empty;   // "Weight", "Color"
    public string Value { get; set; } = string.Empty;  // "1kg", "Red"
    
    // Navigation
    public Product Product { get; set; } = null!;
}
```

---

## Shopping

### Cart
```csharp
public class Cart : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }  // FK, unique
    
    // Navigation
    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
```

### CartItem
```csharp
public class CartItem : AuditableEntity
{
    public int Id { get; set; }
    public int CartId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }            // Price snapshot
    
    // Computed property
    public decimal Subtotal => Quantity * Price;
    
    // Navigation
    public Cart Cart { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
```

---

## Orders

### Order
```csharp
public class Order : AuditableEntity
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;  // unique
    public int UserId { get; set; }
    public int ShippingAddressId { get; set; }
    
    // Status
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // Amounts (stored for historical accuracy)
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }              // 18% GST
    public decimal Discount { get; set; }
    public decimal ShippingCharge { get; set; }
    public decimal Total { get; set; }
    
    // Payment
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
    public PaymentMethod PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }
    public DateTime? PaidAt { get; set; }
    
    // Coupon
    public int? CouponId { get; set; }
    
    // Notes
    public string? CustomerNotes { get; set; }
    public string? AdminNotes { get; set; }
    
    // Tracking
    public DateTime? DeliveredAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public Address ShippingAddress { get; set; } = null!;
    public Coupon? Coupon { get; set; }
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    public ICollection<OrderStatusHistory> StatusHistory { get; set; } = new List<OrderStatusHistory>();
}
```

### OrderItem
```csharp
public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    
    // Product snapshot (for historical accuracy)
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSKU { get; set; }
    public string? ProductImageUrl { get; set; }
    
    // Pricing
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }        // Price snapshot
    
    // Commission (snapshot)
    public decimal CommissionRate { get; set; }   // 5.00 = 5%
    
    // Vendor Status
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    // Computed properties
    public decimal Subtotal => Quantity * UnitPrice;
    public decimal CommissionAmount => Subtotal * (CommissionRate / 100);
    public decimal VendorAmount => Subtotal - CommissionAmount;
    
    // Navigation
    public Order Order { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public VendorProfile Vendor { get; set; } = null!;
}
```

### OrderStatusHistory
```csharp
public class OrderStatusHistory
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public OrderStatus Status { get; set; }
    public string? Notes { get; set; }
    public int? ChangedBy { get; set; }           // FK to User
    public DateTime ChangedAt { get; set; }
    
    // Navigation
    public Order Order { get; set; } = null!;
}
```

---

## Reviews & Social

### Review
```csharp
public class Review : AuditableEntity
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int? OrderItemId { get; set; }         // Link to purchase
    
    // Review content
    public int Rating { get; set; }               // 1-5 stars
    public string? Title { get; set; }
    public string Comment { get; set; } = string.Empty;
    public string? ImageUrls { get; set; }        // Comma-separated or JSON
    
    // Verification & Moderation
    public bool IsVerifiedPurchase { get; set; }
    public bool IsApproved { get; set; } = true;
    public DateTime? ApprovedAt { get; set; }
    public int? ApprovedBy { get; set; }
    
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
```

### Wishlist
```csharp
public class Wishlist : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }               // FK
    public int ProductId { get; set; }            // FK
    
    // Navigation
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
```

---

## Coupons & Payouts

### Coupon
```csharp
public class Coupon : AuditableEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;  // unique
    public string? Description { get; set; }
    public CouponType Type { get; set; }
    
    // Discount values
    public decimal? DiscountPercentage { get; set; }
    public decimal? DiscountAmount { get; set; }
    
    // Constraints
    public decimal? MinOrderValue { get; set; }
    public decimal? MaxDiscount { get; set; }
    
    // Validity
    public DateTime ValidFrom { get; set; }
    public DateTime ValidUntil { get; set; }
    
    // Usage limits
    public int? UsageLimit { get; set; }
    public int UsageCount { get; set; }
    public int? UsageLimitPerUser { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    
    // Computed property
    public bool IsValid => IsActive 
                        && DateTime.UtcNow >= ValidFrom 
                        && DateTime.UtcNow <= ValidUntil
                        && (!UsageLimit.HasValue || UsageCount < UsageLimit.Value);
    
    // Navigation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}
```

### VendorPayout
```csharp
public class VendorPayout : AuditableEntity
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public string PayoutNumber { get; set; } = string.Empty;  // unique
    
    // Period
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    
    // Financial breakdown
    public decimal TotalSales { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal DeductionsAmount { get; set; }
    public decimal NetPayout { get; set; }
    
    // Payment details
    public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? TransactionReference { get; set; }
    
    // Processing
    public int? ProcessedBy { get; set; }
    
    // Computed property
    public decimal CalculatedNetPayout => TotalSales - CommissionAmount - DeductionsAmount;
    
    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
    public User? ProcessedByUser { get; set; }
}
```

---

## Subscriptions (PRIVATE) - Multi-Item Support

### Subscription
```csharp
public class Subscription : AuditableEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VendorId { get; set; }              // Single vendor per subscription
    public int DeliveryAddressId { get; set; }
    
    public string SubscriptionNumber { get; set; } = string.Empty;  // unique
    
    // Subscription settings (SAME for all items)
    public SubscriptionFrequency Frequency { get; set; }
    public int? CustomFrequencyDays { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime NextDeliveryDate { get; set; }
    public string? PreferredDeliveryTime { get; set; }  // "08:00"
    
    // Billing
    public int BillingCycleDays { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public decimal UnpaidAmount { get; set; }
    public decimal CreditLimit { get; set; } = 1200m;
    
    // Deposit (for entire subscription)
    public decimal? DepositAmount { get; set; }
    public decimal? DepositPaid { get; set; }
    public decimal? DepositBalance { get; set; }
    public DateTime? DepositPaidAt { get; set; }
    
    // Status
    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime? PausedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    
    // Statistics
    public int TotalDeliveries { get; set; }
    public int CompletedDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    
    // Navigation
    public User User { get; set; } = null!;
    public VendorProfile Vendor { get; set; } = null!;
    public Address DeliveryAddress { get; set; } = null!;
    public ICollection<SubscriptionItem> Items { get; set; } = new List<SubscriptionItem>();
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
    public ICollection<SubscriptionInvoice> Invoices { get; set; } = new List<SubscriptionInvoice>();
}
```

### SubscriptionItem (NEW)
```csharp
public class SubscriptionItem
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public int ProductId { get; set; }
    
    // Item-specific settings
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }         // Snapshot at subscription time
    public decimal? DiscountPercentage { get; set; }
    
    // Computed
    public decimal ItemTotal
    {
        get
        {
            var total = UnitPrice * Quantity;
            if (DiscountPercentage.HasValue)
                total -= total * (DiscountPercentage.Value / 100);
            return total;
        }
    }
    
    // Navigation
    public Subscription Subscription { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
```

### Delivery
```csharp
public class Delivery : AuditableEntity
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public int? InvoiceId { get; set; }
    
    public string DeliveryNumber { get; set; } = string.Empty;
    public DateTime ScheduledDate { get; set; }
    public DateTime? ActualDeliveryDate { get; set; }
    
    public DeliveryStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    
    // Total for entire delivery (all items)
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? PaymentTransactionId { get; set; }
    
    public string? DeliveryPersonName { get; set; }
    public string? FailureReason { get; set; }
    
    // Navigation
    public Subscription Subscription { get; set; } = null!;
    public SubscriptionInvoice? Invoice { get; set; }
    public ICollection<DeliveryItem> Items { get; set; } = new List<DeliveryItem>();
}
```

### DeliveryItem (NEW)
```csharp
public class DeliveryItem
{
    public int Id { get; set; }
    public int DeliveryId { get; set; }
    public int SubscriptionItemId { get; set; }
    public int ProductId { get; set; }
    
    // Snapshot
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    
    // Status (per-item for partial deliveries)
    public DeliveryItemStatus Status { get; set; } = DeliveryItemStatus.Scheduled;
    public string? Notes { get; set; }  // "Out of stock", "Delivered damaged"
    
    // Navigation
    public Delivery Delivery { get; set; } = null!;
    public SubscriptionItem SubscriptionItem { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
```

### SubscriptionInvoice
```csharp
public class SubscriptionInvoice : AuditableEntity
{
    public int Id { get; set; }
    public int SubscriptionId { get; set; }
    public int UserId { get; set; }
    public int VendorId { get; set; }
    
    public string InvoiceNumber { get; set; } = string.Empty;  // unique
    public DateTime GeneratedAt { get; set; }
    public DateTime DueDate { get; set; }
    
    // Period covered
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int TotalDeliveries { get; set; }
    
    // Amounts
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    
    // Status
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Generated;
    
    // Payment details
    public DateTime? PaidAt { get; set; }
    public string? PaymentMethod { get; set; }
    public string? PaymentTransactionId { get; set; }
    
    // Generation tracking
    public bool IsManuallyGenerated { get; set; }
    
    // Computed properties
    public decimal BalanceDue => Total - PaidAmount;
    public bool IsFullyPaid => PaidAmount >= Total;
    public bool IsOverdue => Status == InvoiceStatus.Overdue || 
                             (Status != InvoiceStatus.Paid && DateTime.UtcNow > DueDate);
    
    // Navigation
    public Subscription Subscription { get; set; } = null!;
    public User User { get; set; } = null!;
    public VendorProfile Vendor { get; set; } = null!;
    public ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();
}
```

---

## Indexes

```sql
-- Users
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
CREATE UNIQUE INDEX IX_Users_PhoneNumber ON Users(PhoneNumber);

-- VendorProfiles
CREATE UNIQUE INDEX IX_VendorProfiles_UserId ON VendorProfiles(UserId);
CREATE UNIQUE INDEX IX_VendorProfiles_GstNumber ON VendorProfiles(GstNumber);

-- Addresses
CREATE INDEX IX_Addresses_UserId ON Addresses(UserId);

-- Categories
CREATE UNIQUE INDEX IX_Categories_Slug ON Categories(Slug);
CREATE INDEX IX_Categories_ParentCategoryId ON Categories(ParentCategoryId);

-- Products
CREATE UNIQUE INDEX IX_Products_Slug ON Products(Slug);
CREATE UNIQUE INDEX IX_Products_SKU ON Products(SKU) WHERE SKU IS NOT NULL;
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId);
CREATE INDEX IX_Products_VendorId ON Products(VendorId);
CREATE INDEX IX_Products_Status ON Products(Status);
CREATE INDEX IX_Products_VendorId_Status ON Products(VendorId, Status);
CREATE INDEX IX_Products_CategoryId_Status ON Products(CategoryId, Status);

-- ProductImages
CREATE INDEX IX_ProductImages_ProductId ON ProductImages(ProductId);

-- ProductSpecifications
CREATE INDEX IX_ProductSpecifications_ProductId ON ProductSpecifications(ProductId);

-- Carts
CREATE UNIQUE INDEX IX_Carts_UserId ON Carts(UserId);

-- CartItems
CREATE INDEX IX_CartItems_CartId ON CartItems(CartId);
CREATE INDEX IX_CartItems_ProductId ON CartItems(ProductId);

-- Orders
CREATE UNIQUE INDEX IX_Orders_OrderNumber ON Orders(OrderNumber);
CREATE INDEX IX_Orders_UserId ON Orders(UserId);
CREATE INDEX IX_Orders_Status ON Orders(Status);
CREATE INDEX IX_Orders_CreatedAt ON Orders(CreatedAt);
CREATE INDEX IX_Orders_ShippingAddressId ON Orders(ShippingAddressId);

-- OrderItems
CREATE INDEX IX_OrderItems_OrderId ON OrderItems(OrderId);
CREATE INDEX IX_OrderItems_ProductId ON OrderItems(ProductId);
CREATE INDEX IX_OrderItems_VendorId ON OrderItems(VendorId);
CREATE INDEX IX_OrderItems_VendorId_Status ON OrderItems(VendorId, Status);

-- OrderStatusHistory
CREATE INDEX IX_OrderStatusHistory_OrderId ON OrderStatusHistory(OrderId);

-- Reviews
CREATE INDEX IX_Reviews_ProductId ON Reviews(ProductId);
CREATE INDEX IX_Reviews_UserId ON Reviews(UserId);
CREATE UNIQUE INDEX IX_Reviews_ProductId_UserId ON Reviews(ProductId, UserId);

-- Wishlists
CREATE INDEX IX_Wishlists_UserId ON Wishlists(UserId);
CREATE UNIQUE INDEX IX_Wishlists_UserId_ProductId ON Wishlists(UserId, ProductId);

-- Coupons
CREATE UNIQUE INDEX IX_Coupons_Code ON Coupons(Code);
CREATE INDEX IX_Coupons_IsActive ON Coupons(IsActive);
CREATE INDEX IX_Coupons_ValidFrom_ValidUntil ON Coupons(ValidFrom, ValidUntil);

-- VendorPayouts
CREATE UNIQUE INDEX IX_VendorPayouts_PayoutNumber ON VendorPayouts(PayoutNumber);
CREATE INDEX IX_VendorPayouts_VendorId ON VendorPayouts(VendorId);
CREATE INDEX IX_VendorPayouts_Status ON VendorPayouts(Status);

-- Subscriptions
CREATE UNIQUE INDEX IX_Subscriptions_SubscriptionNumber ON Subscriptions(SubscriptionNumber);
CREATE INDEX IX_Subscriptions_UserId ON Subscriptions(UserId);
CREATE INDEX IX_Subscriptions_VendorId ON Subscriptions(VendorId);
CREATE INDEX IX_Subscriptions_Status ON Subscriptions(Status);
CREATE INDEX IX_Subscriptions_NextBillingDate ON Subscriptions(NextBillingDate);
CREATE INDEX IX_Subscriptions_NextDeliveryDate ON Subscriptions(NextDeliveryDate);

-- SubscriptionItems
CREATE INDEX IX_SubscriptionItems_SubscriptionId ON SubscriptionItems(SubscriptionId);
CREATE INDEX IX_SubscriptionItems_ProductId ON SubscriptionItems(ProductId);

-- Deliveries
CREATE UNIQUE INDEX IX_Deliveries_DeliveryNumber ON Deliveries(DeliveryNumber);
CREATE INDEX IX_Deliveries_SubscriptionId ON Deliveries(SubscriptionId);
CREATE INDEX IX_Deliveries_ScheduledDate ON Deliveries(ScheduledDate);
CREATE INDEX IX_Deliveries_Status ON Deliveries(Status);
CREATE INDEX IX_Deliveries_InvoiceId ON Deliveries(InvoiceId);

-- DeliveryItems
CREATE INDEX IX_DeliveryItems_DeliveryId ON DeliveryItems(DeliveryId);
CREATE INDEX IX_DeliveryItems_SubscriptionItemId ON DeliveryItems(SubscriptionItemId);

-- SubscriptionInvoices
CREATE UNIQUE INDEX IX_SubscriptionInvoices_InvoiceNumber ON SubscriptionInvoices(InvoiceNumber);
CREATE INDEX IX_SubscriptionInvoices_SubscriptionId ON SubscriptionInvoices(SubscriptionId);
CREATE INDEX IX_SubscriptionInvoices_Status ON SubscriptionInvoices(Status);
CREATE INDEX IX_SubscriptionInvoices_DueDate ON SubscriptionInvoices(DueDate);
CREATE INDEX IX_SubscriptionInvoices_UserId_Status ON SubscriptionInvoices(UserId, Status);
```

---

## Key Constraints

1. **Email & Phone:** Unique per user
2. **GST Number:** Unique per vendor
3. **Product SKU:** Unique across all products (nullable)
4. **Order Number:** Auto-generated, unique (ORD-2025-0124-001)
5. **Subscription Number:** Auto-generated, unique (SUB-2025-0124-001)
6. **Delivery Number:** Auto-generated, unique (DEL-2025-0124-001)
7. **Invoice Number:** Auto-generated, unique (INV-2025-0124-001)
8. **Payout Number:** Auto-generated, unique (PAYOUT-2025-0124-001)
9. **Soft Deletes:** Use IsDeleted flag, apply query filters via `HasQueryFilter(e => !e.IsDeleted)`
10. **Audit Fields:** CreatedAt, UpdatedAt, CreatedBy, UpdatedBy, IsDeleted on all AuditableEntity
11. **Precision:** Decimal(18,2) for all money fields, Decimal(5,2) for percentages
12. **Foreign Keys:** All have proper ON DELETE constraints (Restrict/Cascade as appropriate)
13. **One Subscription = One Vendor:** All items in a subscription must be from the same vendor
14. **One Subscription = One Frequency:** All items share the parent subscription's delivery schedule

---

## Multi-Item Subscription Example

```
Customer creates subscription: "Daily Dairy Bundle"

Subscription:
├─ VendorId: 10 (Fresh Dairy)
├─ Frequency: Daily
├─ BillingCycleDays: 30
└─ Items:
    ├─ SubscriptionItem 1: Milk 1L × 1 = ₹60
    ├─ SubscriptionItem 2: Yogurt 500g × 1 = ₹40
    └─ SubscriptionItem 3: Paneer 200g × 2 = ₹180

Daily Delivery (Jan 25):
├─ DeliveryItem 1: Milk 1L - Delivered ✓
├─ DeliveryItem 2: Yogurt 500g - Delivered ✓
└─ DeliveryItem 3: Paneer 200g - Out of Stock ✗

Total charged: ₹100 (only delivered items)
Unpaid amount += ₹100

After 30 days → Invoice generated for ₹3000 (30 days × ₹100)
```

---

## Sample Data

```sql
-- Categories
Dairy, Beverages, Groceries, Vegetables

-- Products
Full Cream Milk (1L) - ₹60
Bisleri Water (20L) - ₹80
Aashirvaad Atta (5kg) - ₹250
Amul Paneer (200g) - ₹90
Fresh Yogurt (500g) - ₹40
```

---

## Table Summary

**Public E-Commerce (16 tables):**
1. User
2. VendorProfile
3. Address
4. Category
5. Product
6. ProductImage
7. ProductSpecification
8. Cart
9. CartItem
10. Order
11. OrderItem
12. OrderStatusHistory
13. Review
14. Wishlist
15. Coupon
16. VendorPayout

**Private Subscription (5 tables):**
17. Subscription
18. SubscriptionItem
19. Delivery
20. DeliveryItem
21. SubscriptionInvoice

**Total: 21 tables**
