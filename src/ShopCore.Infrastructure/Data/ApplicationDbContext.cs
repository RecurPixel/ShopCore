using Microsoft.EntityFrameworkCore;
using ShopCore.Application.Common.Interfaces;
using ShopCore.Domain.Entities;
using ShopCore.Infrastructure.EntityConfigurations;
using CartEntity = ShopCore.Domain.Entities.Cart;
using WishlistEntity = ShopCore.Domain.Entities.Wishlist;

namespace ShopCore.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<VendorProfile> VendorProfiles { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductSpecification> ProductSpecifications { get; set; }
    public DbSet<CartEntity> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<SubscriptionItem> SubscriptionItems { get; set; }
    public DbSet<Delivery> Deliveries { get; set; }
    public DbSet<DeliveryItem> DeliveryItems { get; set; }
    public DbSet<SubscriptionInvoice> SubscriptionInvoices { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<VendorPayout> VendorPayouts { get; set; }
    public DbSet<WishlistEntity> Wishlists { get; set; }
    public DbSet<Coupon> Coupons { get; set; }
    public DbSet<VendorServiceArea> VendorServiceAreas { get; set; }
    public DbSet<CustomerInvitation> CustomerInvitations { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationLog> NotificationLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new VendorProfileConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new ProductImageConfiguration());
        modelBuilder.ApplyConfiguration(new ProductSpecificationConfiguration());
        modelBuilder.ApplyConfiguration(new CartConfiguration());
        modelBuilder.ApplyConfiguration(new CartItemConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new OrderStatusHistoryConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionItemConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryItemConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionInvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new ReviewConfiguration());
        modelBuilder.ApplyConfiguration(new WishlistConfiguration());
        modelBuilder.ApplyConfiguration(new CouponConfiguration());
        modelBuilder.ApplyConfiguration(new VendorPayoutConfiguration());
        modelBuilder.ApplyConfiguration(new VendorServiceAreaConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerInvitationConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationLogConfiguration());

        // Global query filters (soft delete)
        modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<VendorProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Address>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Product>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ProductImage>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CartEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<CartItem>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Subscription>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Delivery>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<SubscriptionInvoice>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<VendorPayout>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<WishlistEntity>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Coupon>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Notification>().HasQueryFilter(e => !e.IsDeleted);
    }

    // Override SaveChanges to automatically set timestamps
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                // Set CreatedAt for new entities
                try
                {
                    entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (InvalidOperationException)
                {
                    // Property doesn't exist, skip
                }
            }

            // Set UpdatedAt for modified entities
            if (entry.State == EntityState.Modified)
            {
                try
                {
                    entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (InvalidOperationException)
                {
                    // Property doesn't exist, skip
                }
            }

            if (entry.State == EntityState.Deleted)
            {
                try
                {
                    entry.Property("DeletedAt").CurrentValue = DateTime.UtcNow;
                }
                catch (InvalidOperationException)
                {
                    // Property doesn't exist, skip
                }
            }
        }
    }
}
