using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class DeliveryItemConfiguration : IEntityTypeConfiguration<DeliveryItem>
{
    public void Configure(EntityTypeBuilder<DeliveryItem> builder)
    {
        builder.HasKey(di => di.Id);

        builder.Property(di => di.ProductName).IsRequired().HasMaxLength(200);

        builder.Property(di => di.UnitPrice).HasPrecision(18, 2);

        builder.Property(di => di.Amount).HasPrecision(18, 2);

        builder.Property(di => di.Status).HasConversion<int>();

        builder.Property(di => di.Notes).HasMaxLength(500);

        // Indexes
        builder.HasIndex(di => di.DeliveryId);

        builder.HasIndex(di => di.SubscriptionItemId);

        // Relationships
        builder
            .HasOne(di => di.Delivery)
            .WithMany(d => d.Items)
            .HasForeignKey(di => di.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(di => di.SubscriptionItem)
            .WithMany()
            .HasForeignKey(di => di.SubscriptionItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(di => di.Product)
            .WithMany()
            .HasForeignKey(di => di.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
