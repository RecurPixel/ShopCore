using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber).IsUnique();

        builder.Property(o => o.Subtotal).HasPrecision(18, 2);

        builder.Property(o => o.Tax).HasPrecision(18, 2);

        builder.Property(o => o.Discount).HasPrecision(18, 2);

        builder.Property(o => o.ShippingCharge).HasPrecision(18, 2);

        builder.Property(o => o.Total).HasPrecision(18, 2);

        builder.Property(o => o.RefundedAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(o => o.PaymentStatus).HasConversion<int>();

        builder.Property(o => o.PaymentMethod).HasConversion<int>();

        builder.Property(o => o.PaymentTransactionId).HasMaxLength(100);

        builder.Property(o => o.Status).HasConversion<int>();

        builder.Property(o => o.CustomerNotes).HasMaxLength(1000);

        builder.Property(o => o.AdminNotes).HasMaxLength(1000);

        builder.Property(o => o.CancellationReason).HasMaxLength(500);

        // Indexes
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);
        builder.HasIndex(o => o.ShippingAddressId);

        // Relationships
        builder
            .HasOne(o => o.ShippingAddress)
            .WithMany(a => a.Orders)
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(o => o.Coupon)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CouponId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(o => o.StatusHistory)
            .WithOne(h => h.Order)
            .HasForeignKey(h => h.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
