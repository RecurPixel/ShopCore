# ShopCore - Implementation Changes Checklist

**Use this to update your codebase with all discussed changes**

---

## 📊 DATABASE CHANGES

### NEW ENTITIES (2)

#### 1. VendorServiceArea
```csharp
public class VendorServiceArea : AuditableEntity
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    
    // Area definition
    public string AreaName { get; set; } = string.Empty;  // "Koramangala"
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public List<string> Pincodes { get; set; } = new();  // JSON: ["560034", "560035"]
    
    // Optional geofencing (Phase 2)
    public string? GeoJsonPolygon { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
}
```

#### 2. CustomerInvitation
```csharp
public class CustomerInvitation : AuditableEntity
{
    public int Id { get; set; }
    public int VendorId { get; set; }
    public string InvitationToken { get; set; } = string.Empty;  // Unique
    
    // Customer details
    public string CustomerName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    
    // Subscription details (pre-filled by vendor)
    public string SubscriptionItemsJson { get; set; } = string.Empty;  // JSON array
    public SubscriptionFrequency Frequency { get; set; }
    public string PreferredDeliveryTime { get; set; } = string.Empty;
    public decimal? DepositAmount { get; set; }
    
    // Status
    public InvitationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public int? UserId { get; set; }  // Set after acceptance
    
    // Navigation
    public VendorProfile Vendor { get; set; } = null!;
    public User? User { get; set; }
}

// Helper class for JSON serialization
public class InvitationSubscriptionItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
```

---

### MODIFIED ENTITIES (8)

#### 1. Address - Add Location Fields
```csharp
public class Address : AuditableEntity
{
    // ... existing fields ...
    
    // ADD THESE:
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? PlaceId { get; set; }  // Google Maps Place ID
    public AddressType Type { get; set; } = AddressType.Home;
    public string? Landmark { get; set; }
}
```

#### 2. Order - Add Refund Tracking & Status Logic
```csharp
public class Order : AuditableEntity
{
    // ... existing fields ...
    
    // ADD THIS:
    public decimal RefundedAmount { get; set; } = 0;
    
    // MODIFY Status property:
    private OrderStatus _status = OrderStatus.Pending;
    public OrderStatus Status 
    { 
        get => _status; 
        private set => _status = value;  // PRIVATE setter!
    }
    
    // ADD THESE METHODS:
    public void UpdateStatusFromItems()
    {
        if (!Items.Any()) 
        {
            _status = OrderStatus.Pending;
            return;
        }
        
        var statuses = Items.Select(i => i.Status).ToList();
        
        // All cancelled
        if (statuses.All(s => s == OrderItemStatus.Cancelled))
        {
            _status = OrderStatus.Cancelled;
            return;
        }
        
        // Some cancelled
        if (statuses.Any(s => s == OrderItemStatus.Cancelled))
        {
            _status = OrderStatus.PartiallyCancelled;
            return;
        }
        
        // All delivered
        if (statuses.All(s => s == OrderItemStatus.Delivered))
        {
            _status = OrderStatus.Delivered;
            return;
        }
        
        // Some delivered
        if (statuses.Any(s => s == OrderItemStatus.Delivered))
        {
            _status = OrderStatus.PartiallyDelivered;
            return;
        }
        
        // All shipped
        if (statuses.All(s => s == OrderItemStatus.Shipped))
        {
            _status = OrderStatus.Shipped;
            return;
        }
        
        // Some shipped
        if (statuses.Any(s => s == OrderItemStatus.Shipped))
        {
            _status = OrderStatus.PartiallyShipped;
            return;
        }
        
        // At least one processing
        if (statuses.Any(s => s == OrderItemStatus.Processing))
        {
            _status = OrderStatus.Processing;
            return;
        }
        
        // All confirmed
        if (statuses.All(s => s == OrderItemStatus.Confirmed))
        {
            _status = OrderStatus.Confirmed;
            return;
        }
        
        _status = OrderStatus.Pending;
    }
}
```

#### 3. OrderItem - Add Tracking Fields
```csharp
public class OrderItem
{
    // ... existing fields ...
    
    // ADD THESE:
    public string? TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? CancellationReason { get; set; }
}
```

#### 4. Subscription - Add One-Time Delivery Support
```csharp
public class Subscription : AuditableEntity
{
    // ... existing fields ...
    
    // ADD THESE:
    public bool IsOneTimeDelivery { get; set; } = false;
    public bool AutoCancelAfterDelivery { get; set; } = false;
}
```

#### 5. SubscriptionItem - Add Recurring/One-Time Support
```csharp
public class SubscriptionItem
{
    // ... existing fields ...
    
    // ADD THESE:
    public bool IsRecurring { get; set; } = true;
    public DateTime? OneTimeDeliveryDate { get; set; }
    public bool IsDelivered { get; set; } = false;
}
```

#### 6. Delivery - Add Driver & Proof Fields (Optional - Phase 2)
```csharp
public class Delivery : AuditableEntity
{
    // ... existing fields ...
    
    // ADD THESE (Optional for Phase 2):
    public int? AssignedDriverId { get; set; }
    public DateTime? AssignedAt { get; set; }
    public string? DeliveryPhotoUrl { get; set; }
    public string? CustomerSignatureUrl { get; set; }
    public string? DeliveryNotes { get; set; }
    
    // Navigation (if adding Driver role)
    // public DriverProfile? AssignedDriver { get; set; }
}
```

#### 7. VendorProfile - Add Navigation
```csharp
public class VendorProfile : AuditableEntity
{
    // ... existing fields ...
    
    // ADD THIS NAVIGATION:
    public ICollection<VendorServiceArea> ServiceAreas { get; set; } = new List<VendorServiceArea>();
    public ICollection<CustomerInvitation> CustomerInvitations { get; set; } = new List<CustomerInvitation>();
}
```

#### 8. User - Add Navigation
```csharp
public class User : AuditableEntity
{
    // ... existing fields ...
    
    // ADD THIS NAVIGATION:
    public ICollection<CustomerInvitation> ReceivedInvitations { get; set; } = new List<CustomerInvitation>();
}
```

---

### NEW/UPDATED ENUMS (5)

#### 1. AddressType (NEW)
```csharp
public enum AddressType
{
    Home = 1,
    Office = 2,
    Other = 3
}
```

#### 2. InvitationStatus (NEW)
```csharp
public enum InvitationStatus
{
    Pending = 1,
    Sent = 2,
    Accepted = 3,
    Rejected = 4,
    Expired = 5
}
```

#### 3. OrderStatus (UPDATE - Add new values)
```csharp
public enum OrderStatus 
{ 
    Pending = 1,
    PaymentFailed = 2,        // ADD
    Confirmed = 3,
    Processing = 4,
    PartiallyShipped = 5,     // ADD
    Shipped = 6,
    PartiallyDelivered = 7,   // ADD
    Delivered = 8,
    Completed = 9,
    Cancelled = 10,
    PartiallyCancelled = 11,  // ADD
    Refunded = 12,
    PartiallyRefunded = 13    // ADD
}
```

#### 4. PaymentStatus (UPDATE - Add new value)
```csharp
public enum PaymentStatus 
{ 
    Unpaid = 1,
    Pending = 2,
    Paid = 3,
    PartiallyRefunded = 4,  // ADD
    Refunded = 5,
    Failed = 6
}
```

#### 5. DeliveryItemStatus (Ensure this exists)
```csharp
public enum DeliveryItemStatus
{
    Scheduled = 1,
    Delivered = 2,
    OutOfStock = 3,
    Damaged = 4,
    Skipped = 5,
    Cancelled = 6
}
```

---

## 🎯 COMMAND/QUERY CHANGES

### ❌ REMOVE THESE COMMANDS

```csharp
// REMOVE - Order status is now derived
public record UpdateOrderStatusCommand(int OrderId, OrderStatus Status) : IRequest;

// REMOVE - Specs now part of Create/Update product
public record AddProductSpecificationCommand(int ProductId, string Name, string Value) : IRequest;
```

---

### ✅ ADD THESE COMMANDS

#### Vendor Service Areas
```csharp
public record AddVendorServiceAreaCommand(
    string AreaName,
    string City,
    string State,
    List<string> Pincodes
) : IRequest<VendorServiceAreaDto>;

public record UpdateVendorServiceAreaCommand(
    int ServiceAreaId,
    string AreaName,
    List<string> Pincodes
) : IRequest<VendorServiceAreaDto>;

public record RemoveVendorServiceAreaCommand(
    int ServiceAreaId
) : IRequest;

public record GetVendorServiceAreasQuery : IRequest<List<VendorServiceAreaDto>>;
```

#### Customer Invitations
```csharp
public record CreateCustomerInvitationCommand(
    string CustomerName,
    string PhoneNumber,
    string? Email,
    string DeliveryAddress,
    string Pincode,
    string? Landmark,
    List<InvitationItemInput> Items,
    SubscriptionFrequency Frequency,
    string PreferredDeliveryTime,
    decimal? DepositAmount,
    bool SendSms,
    bool SendWhatsApp,
    bool SendEmail
) : IRequest<CustomerInvitationDto>;

public record AcceptInvitationCommand(
    string InvitationToken,
    string? Password,
    bool AgreeToTerms
) : IRequest<InvitationAcceptedDto>;

public record RejectInvitationCommand(
    string InvitationToken,
    string Reason
) : IRequest;

public record ResendInvitationCommand(
    int InvitationId
) : IRequest;

public record CancelInvitationCommand(
    int InvitationId
) : IRequest;

public record GetInvitationDetailsQuery(
    string InvitationToken
) : IRequest<InvitationDetailsDto>;

public record GetMyCustomerInvitationsQuery(
    InvitationStatus? Status = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedList<CustomerInvitationDto>>;

// Input model
public record InvitationItemInput(
    int ProductId,
    int Quantity
);
```

#### Order Management (Updated)
```csharp
// REPLACE UpdateOrderStatusCommand with this:
public record UpdateOrderItemStatusCommand(
    int OrderItemId,
    OrderItemStatus NewStatus,
    string? Notes,
    string? TrackingNumber
) : IRequest;

// ADD:
public record CancelOrderItemCommand(
    int OrderItemId,
    string Reason
) : IRequest<CancellationResultDto>;

// ADD (internal/system use):
public record ProcessRefundCommand(
    int OrderId,
    List<int> OrderItemIds,
    string Reason
) : IRequest<RefundResultDto>;

// KEEP existing:
public record CancelOrderCommand(int OrderId, string Reason) : IRequest<CancellationResultDto>;
```

#### Subscription (One-Time Items)
```csharp
// ADD:
public record AddOneTimeItemToSubscriptionCommand(
    int SubscriptionId,
    int ProductId,
    int Quantity,
    DateTime DeliveryDate,
    PaymentOption Payment
) : IRequest<SubscriptionItemDto>;

// ADD:
public record CreateOneTimeDeliveryCommand(
    int VendorId,
    int DeliveryAddressId,
    List<OrderItemInput> Items,
    DateTime DeliveryDate,
    PaymentOption Payment
) : IRequest<OneTimeDeliveryDto>;

// ADD:
public record ConvertToSubscriptionCommand(
    int OneTimeSubscriptionId,
    SubscriptionFrequency Frequency,
    int BillingCycleDays
) : IRequest<SubscriptionDto>;

// Input models
public record OrderItemInput(
    int ProductId,
    int Quantity
);

public enum PaymentOption
{
    AddToBill = 1,
    PayNow = 2,
    PayOnDelivery = 3
}
```

#### Location Services
```csharp
public record SearchVendorsByLocationQuery(
    string? Pincode = null,
    double? Latitude = null,
    double? Longitude = null,
    int? CategoryId = null,
    int RadiusKm = 5
) : IRequest<List<VendorSearchResultDto>>;

// Update existing CreateAddressCommand to include:
public record CreateAddressCommand(
    string FullName,
    string PhoneNumber,
    string AddressLine1,
    string? AddressLine2,
    string City,
    string State,
    string Pincode,
    double? Latitude,      // ADD
    double? Longitude,     // ADD
    string? PlaceId,       // ADD
    AddressType Type,      // ADD
    string? Landmark       // ADD
) : IRequest<AddressDto>;
```

#### Delivery Management (Updated)
```csharp
// UPDATE existing CompleteDeliveryCommand:
public record CompleteDeliveryCommand(
    int DeliveryId,
    List<DeliveryItemStatusInput> ItemStatuses,
    string? PaymentMethod,
    string? PaymentTransactionId,
    IFormFile? DeliveryPhoto  // ADD
) : IRequest;

public record DeliveryItemStatusInput(
    int DeliveryItemId,
    DeliveryItemStatus Status,
    string? Notes
);
```

---

### 🔄 MODIFY THESE COMMANDS

#### Product Commands - Add Specifications
```csharp
// MODIFY CreateProductCommand - add this parameter:
public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    decimal? CompareAtPrice,
    int CategoryId,
    int StockQuantity,
    string? SKU,
    bool IsSubscriptionAvailable,
    decimal? SubscriptionDiscount,
    List<ProductSpecInput>? Specifications  // ADD THIS
) : IRequest<ProductDto>;

// MODIFY UpdateProductCommand - add this parameter:
public record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    decimal Price,
    decimal? CompareAtPrice,
    int CategoryId,
    int StockQuantity,
    string? SKU,
    bool IsSubscriptionAvailable,
    decimal? SubscriptionDiscount,
    List<ProductSpecInput>? Specifications  // ADD THIS
) : IRequest<ProductDto>;

// ADD input model:
public record ProductSpecInput(
    string Name,
    string Value
);
```

---

## 📋 DTOs TO ADD

```csharp
// Vendor Service Area
public class VendorServiceAreaDto
{
    public int Id { get; set; }
    public string AreaName { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public List<string> Pincodes { get; set; }
    public bool IsActive { get; set; }
}

// Customer Invitation
public class CustomerInvitationDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public string DeliveryAddress { get; set; }
    public string Pincode { get; set; }
    public InvitationStatus Status { get; set; }
    public DateTime SentAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}

public class InvitationDetailsDto
{
    public string VendorName { get; set; }
    public string VendorLogo { get; set; }
    public List<InvitationItemDto> Items { get; set; }
    public string DeliveryTime { get; set; }
    public decimal MonthlyEstimate { get; set; }
    public decimal? DepositAmount { get; set; }
}

public class InvitationItemDto
{
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string Frequency { get; set; }
}

public class InvitationAcceptedDto
{
    public int UserId { get; set; }
    public int SubscriptionId { get; set; }
    public string Message { get; set; }
    public string AppDownloadLink { get; set; }
}

// Order
public class CancellationResultDto
{
    public int OrderId { get; set; }
    public int? OrderItemId { get; set; }
    public DateTime CancelledAt { get; set; }
    public decimal RefundAmount { get; set; }
    public string RefundStatus { get; set; }
}

public class RefundResultDto
{
    public int OrderId { get; set; }
    public decimal RefundAmount { get; set; }
    public string? RefundTransactionId { get; set; }
    public DateTime RefundedAt { get; set; }
    public string Status { get; set; }
}

// Subscription
public class OneTimeDeliveryDto
{
    public int SubscriptionId { get; set; }
    public int DeliveryId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

// Location
public class VendorSearchResultDto
{
    public int VendorId { get; set; }
    public string BusinessName { get; set; }
    public string? BusinessDescription { get; set; }
    public string? LogoUrl { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalProducts { get; set; }
    public List<string> ServingAreas { get; set; }
}
```

---

## 🗄️ ENTITY CONFIGURATIONS

### Add these EF Core configurations:

```csharp
// VendorServiceAreaConfiguration.cs
public class VendorServiceAreaConfiguration : IEntityTypeConfiguration<VendorServiceArea>
{
    public void Configure(EntityTypeBuilder<VendorServiceArea> builder)
    {
        builder.ToTable("VendorServiceAreas");
        
        builder.HasKey(vsa => vsa.Id);
        
        builder.Property(vsa => vsa.AreaName)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(vsa => vsa.Pincodes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null) ?? new List<string>()
            )
            .HasColumnType("nvarchar(max)");
        
        builder.HasOne(vsa => vsa.Vendor)
            .WithMany(v => v.ServiceAreas)
            .HasForeignKey(vsa => vsa.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(vsa => vsa.VendorId);
        builder.HasQueryFilter(vsa => !vsa.IsDeleted);
    }
}

// CustomerInvitationConfiguration.cs
public class CustomerInvitationConfiguration : IEntityTypeConfiguration<CustomerInvitation>
{
    public void Configure(EntityTypeBuilder<CustomerInvitation> builder)
    {
        builder.ToTable("CustomerInvitations");
        
        builder.HasKey(ci => ci.Id);
        
        builder.Property(ci => ci.InvitationToken)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(ci => ci.InvitationToken)
            .IsUnique();
        
        builder.Property(ci => ci.CustomerName)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(ci => ci.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);
        
        builder.HasOne(ci => ci.Vendor)
            .WithMany(v => v.CustomerInvitations)
            .HasForeignKey(ci => ci.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(ci => ci.User)
            .WithMany(u => u.ReceivedInvitations)
            .HasForeignKey(ci => ci.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(ci => ci.VendorId);
        builder.HasIndex(ci => ci.Status);
        builder.HasIndex(ci => ci.ExpiresAt);
        builder.HasQueryFilter(ci => !ci.IsDeleted);
    }
}
```

### Update existing configurations:

```csharp
// AddressConfiguration.cs - ADD these properties:
builder.Property(a => a.Latitude)
    .HasPrecision(10, 7);

builder.Property(a => a.Longitude)
    .HasPrecision(10, 7);

builder.Property(a => a.PlaceId)
    .HasMaxLength(255);

builder.Property(a => a.Landmark)
    .HasMaxLength(200);

// OrderConfiguration.cs - ADD:
builder.Property(o => o.RefundedAmount)
    .HasPrecision(18, 2)
    .HasDefaultValue(0);

// OrderItemConfiguration.cs - ADD:
builder.Property(oi => oi.TrackingNumber)
    .HasMaxLength(100);

builder.Property(oi => oi.CancellationReason)
    .HasMaxLength(500);
```

---

## 📝 VALIDATION UPDATES

### Add these validators:

```csharp
// CreateCustomerInvitationCommandValidator.cs
public class CreateCustomerInvitationCommandValidator : AbstractValidator<CreateCustomerInvitationCommand>
{
    public CreateCustomerInvitationCommandValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .MaximumLength(200);
        
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^[6-9]\d{9}$")
            .WithMessage("Phone number must be a valid Indian mobile number");
        
        RuleFor(x => x.Pincode)
            .NotEmpty()
            .Matches(@"^\d{6}$")
            .WithMessage("Pincode must be 6 digits");
        
        RuleFor(x => x.Items)
            .NotEmpty()
            .WithMessage("At least one item is required");
        
        RuleFor(x => x.PreferredDeliveryTime)
            .NotEmpty()
            .Matches(@"^([01]\d|2[0-3]):([0-5]\d)$")
            .WithMessage("Time must be in HH:mm format");
    }
}

// AddVendorServiceAreaCommandValidator.cs
public class AddVendorServiceAreaCommandValidator : AbstractValidator<AddVendorServiceAreaCommand>
{
    public AddVendorServiceAreaCommandValidator()
    {
        RuleFor(x => x.AreaName)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.Pincodes)
            .NotEmpty()
            .WithMessage("At least one pincode is required")
            .Must(pincodes => pincodes.All(p => Regex.IsMatch(p, @"^\d{6}$")))
            .WithMessage("All pincodes must be 6 digits");
    }
}
```

---

## 🔧 MIGRATION SCRIPT

```bash
# Create migration for all changes
dotnet ef migrations add AddLocationAndInvitationFeatures --project src/ShopCore.Infrastructure --startup-project src/ShopCore.API

# Review migration
# Then apply:
dotnet ef database update --project src/ShopCore.Infrastructure --startup-project src/ShopCore.API
```

---

## ✅ IMPLEMENTATION CHECKLIST

### Step 1: Domain Layer
- [ ] Add new enums (AddressType, InvitationStatus)
- [ ] Update existing enums (OrderStatus, PaymentStatus)
- [ ] Create VendorServiceArea entity
- [ ] Create CustomerInvitation entity
- [ ] Update Address entity (location fields)
- [ ] Update Order entity (RefundedAmount, UpdateStatusFromItems method)
- [ ] Update OrderItem entity (tracking fields)
- [ ] Update Subscription entity (one-time flags)
- [ ] Update SubscriptionItem entity (recurring fields)
- [ ] Update Delivery entity (optional driver fields)
- [ ] Update VendorProfile navigation
- [ ] Update User navigation

### Step 2: Infrastructure Layer
- [ ] Create VendorServiceAreaConfiguration
- [ ] Create CustomerInvitationConfiguration
- [ ] Update AddressConfiguration
- [ ] Update OrderConfiguration
- [ ] Update OrderItemConfiguration
- [ ] Update SubscriptionConfiguration
- [ ] Update SubscriptionItemConfiguration
- [ ] Update DbContext (add DbSets)
- [ ] Create and run migration

### Step 3: Application Layer - Commands
- [ ] Remove UpdateOrderStatusCommand
- [ ] Remove AddProductSpecificationCommand
- [ ] Add UpdateOrderItemStatusCommand + Handler + Validator
- [ ] Add CancelOrderItemCommand + Handler + Validator
- [ ] Add ProcessRefundCommand + Handler
- [ ] Add AddVendorServiceAreaCommand + Handler + Validator
- [ ] Add UpdateVendorServiceAreaCommand + Handler + Validator
- [ ] Add RemoveVendorServiceAreaCommand + Handler
- [ ] Add CreateCustomerInvitationCommand + Handler + Validator
- [ ] Add AcceptInvitationCommand + Handler + Validator
- [ ] Add RejectInvitationCommand + Handler
- [ ] Add ResendInvitationCommand + Handler
- [ ] Add CancelInvitationCommand + Handler
- [ ] Add AddOneTimeItemToSubscriptionCommand + Handler + Validator
- [ ] Add CreateOneTimeDeliveryCommand + Handler + Validator
- [ ] Add ConvertToSubscriptionCommand + Handler + Validator
- [ ] Update CreateProductCommand (add Specifications parameter)
- [ ] Update UpdateProductCommand (add Specifications parameter)
- [ ] Update CreateAddressCommand (add location parameters)
- [ ] Update CompleteDeliveryCommand (add photo parameter)

### Step 4: Application Layer - Queries
- [ ] Add GetVendorServiceAreasQuery + Handler
- [ ] Add SearchVendorsByLocationQuery + Handler
- [ ] Add GetInvitationDetailsQuery + Handler
- [ ] Add GetMyCustomerInvitationsQuery + Handler

### Step 5: Application Layer - DTOs
- [ ] Add all new DTOs listed above
- [ ] Update existing DTOs if needed

### Step 6: API Layer - Controllers
- [ ] Add VendorServiceAreasController (or add to VendorsController)
- [ ] Add CustomerInvitationsController (or add to VendorsController)
- [ ] Update OrdersController (item cancellation)
- [ ] Update VendorsController (order item status update)
- [ ] Update SubscriptionsController (one-time items)
- [ ] Update DeliveriesController (photo upload)
- [ ] Add location search endpoints

### Step 7: Testing
- [ ] Test order item status updates
- [ ] Test order status derivation
- [ ] Test refund processing
- [ ] Test vendor service areas
- [ ] Test customer invitations
- [ ] Test location search
- [ ] Test one-time items in subscriptions

---

## 🎯 QUICK WINS (Do These First)

**Minimum to get subscription system working:**

1. ✅ Update Order/OrderItem entities
2. ✅ Update Subscription/SubscriptionItem entities
3. ✅ Update order status commands
4. ✅ Add VendorServiceArea entity
5. ✅ Add CustomerInvitation entity
6. ✅ Create migration and update database
7. ✅ Implement UpdateOrderItemStatusCommand
8. ✅ Implement CreateCustomerInvitationCommand
9. ✅ Implement AcceptInvitationCommand
10. ✅ Implement AddOneTimeItemToSubscriptionCommand

**Everything else can come later!**

---

## 📌 NOTES

- Phase 2 items (Driver fields, etc.) are marked as optional
- Master product catalog is deferred - not in this checklist
- All location features use pincodes initially, geofencing is Phase 2
- Focus on getting core flows working before polish

---

**Use this as your implementation guide. Check items off as you complete them!**
