using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200);

        builder.Property(oi => oi.ProductSKU).HasMaxLength(50);

        builder.Property(oi => oi.ProductImageUrl).HasMaxLength(500);

        builder.Property(oi => oi.UnitPrice).HasPrecision(18, 2);

        builder.Property(oi => oi.CommissionRate).HasPrecision(5, 2);

        builder.Property(oi => oi.Status).HasConversion<int>();

        // Tracking fields
        builder.Property(oi => oi.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(oi => oi.CancellationReason)
            .HasMaxLength(500);

        // Ignore computed properties
        builder.Ignore(oi => oi.Subtotal);
        builder.Ignore(oi => oi.CommissionAmount);
        builder.Ignore(oi => oi.VendorAmount);

        // Indexes
        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);
        builder.HasIndex(oi => oi.VendorId);
        builder.HasIndex(oi => new { oi.VendorId, oi.Status });

        // Relationships
        builder
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(oi => oi.Vendor)
            .WithMany()
            .HasForeignKey(oi => oi.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(oi => oi.Product)
            .WithMany(p => p.OrderItems)
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
