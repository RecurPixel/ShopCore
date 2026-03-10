using CartEntity = ShopCore.Domain.Entities.Cart;
using WishlistEntity = ShopCore.Domain.Entities.Wishlist;

namespace ShopCore.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    // Core
    DbSet<User> Users { get; }
    DbSet<VendorProfile> VendorProfiles { get; }
    DbSet<Address> Addresses { get; }

    // Catalog
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<ProductImage> ProductImages { get; }
    DbSet<ProductSpecification> ProductSpecifications { get; }

    // Shopping
    DbSet<CartEntity> Carts { get; }
    DbSet<CartItem> CartItems { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderStatusHistory> OrderStatusHistories { get; }

    // Social
    DbSet<Review> Reviews { get; }
    DbSet<WishlistEntity> Wishlists { get; }

    // Coupons & Payouts
    DbSet<Coupon> Coupons { get; }
    DbSet<VendorPayout> VendorPayouts { get; }

    // Subscriptions (PRIVATE)
    DbSet<Subscription> Subscriptions { get; }
    DbSet<SubscriptionItem> SubscriptionItems { get; }
    DbSet<Delivery> Deliveries { get; }
    DbSet<DeliveryItem> DeliveryItems { get; }
    DbSet<SubscriptionInvoice> SubscriptionInvoices { get; }

    // Location & Onboarding
    DbSet<VendorServiceArea> VendorServiceAreas { get; }
    DbSet<CustomerInvitation> CustomerInvitations { get; }

    // Notifications
    DbSet<Notification> Notifications { get; }
    DbSet<NotificationLog> NotificationLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
