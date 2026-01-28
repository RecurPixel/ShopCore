using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SubscriptionNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(s => s.SubscriptionNumber).IsUnique();

        builder.Property(s => s.Frequency).HasConversion<int>();

        builder.Property(s => s.PreferredDeliveryTime).HasMaxLength(10);

        builder.Property(s => s.UnpaidAmount).HasPrecision(18, 2);

        builder.Property(s => s.CreditLimit).HasPrecision(18, 2);

        builder.Property(s => s.DepositAmount).HasPrecision(18, 2);

        builder.Property(s => s.DepositPaid).HasPrecision(18, 2);

        builder.Property(s => s.DepositBalance).HasPrecision(18, 2);

        builder.Property(s => s.Status).HasConversion<int>();

        // One-Time Delivery Support
        builder.Property(s => s.IsOneTimeDelivery)
            .HasDefaultValue(false);

        builder.Property(s => s.AutoCancelAfterDelivery)
            .HasDefaultValue(false);

        builder.Property(s => s.CancellationReason).HasMaxLength(500);

        // Indexes
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.VendorId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.NextDeliveryDate);
        builder.HasIndex(s => s.NextBillingDate);

        // Relationships
        builder
            .HasOne(s => s.Vendor)
            .WithMany(v => v.Subscriptions)
            .HasForeignKey(s => s.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(s => s.DeliveryAddress)
            .WithMany(a => a.Subscriptions)
            .HasForeignKey(s => s.DeliveryAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(s => s.Items)
            .WithOne(si => si.Subscription)
            .HasForeignKey(si => si.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Deliveries)
            .WithOne(d => d.Subscription)
            .HasForeignKey(d => d.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Invoices)
            .WithOne(i => i.Subscription)
            .HasForeignKey(i => i.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
