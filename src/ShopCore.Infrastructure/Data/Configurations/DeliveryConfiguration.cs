using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.HasKey(d => d.Id);

        builder.Property(d => d.DeliveryNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(d => d.DeliveryNumber).IsUnique();

        builder.Property(d => d.Status).HasConversion<int>();

        builder.Property(d => d.PaymentStatus).HasConversion<int>();

        builder.Property(d => d.TotalAmount).HasPrecision(18, 2);

        builder.Property(d => d.PaymentMethod).HasMaxLength(50);

        builder.Property(d => d.PaymentTransactionId).HasMaxLength(100);

        builder.Property(d => d.DeliveryPersonName).HasMaxLength(100);

        builder.Property(d => d.FailureReason).HasMaxLength(500);

        // Indexes
        builder.HasIndex(d => d.SubscriptionId);
        builder.HasIndex(d => d.ScheduledDate);
        builder.HasIndex(d => d.Status);
        builder.HasIndex(d => d.InvoiceId);

        // Relationships
        builder
            .HasOne(d => d.Invoice)
            .WithMany(i => i.Deliveries)
            .HasForeignKey(d => d.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasMany(d => d.Items)
            .WithOne(di => di.Delivery)
            .HasForeignKey(di => di.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
