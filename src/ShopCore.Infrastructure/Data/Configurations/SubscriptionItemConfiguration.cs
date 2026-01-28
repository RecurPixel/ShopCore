using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class SubscriptionItemConfiguration : IEntityTypeConfiguration<SubscriptionItem>
{
    public void Configure(EntityTypeBuilder<SubscriptionItem> builder)
    {
        builder.HasKey(si => si.Id);

        builder.Property(si => si.UnitPrice).HasPrecision(18, 2);

        builder.Property(si => si.DiscountPercentage).HasPrecision(5, 2);

        // Recurring/One-Time Support
        builder.Property(si => si.IsRecurring)
            .HasDefaultValue(true);

        builder.Property(si => si.IsDelivered)
            .HasDefaultValue(false);

        // Ignore computed property
        builder.Ignore(si => si.ItemTotal);

        // Indexes
        builder.HasIndex(si => si.SubscriptionId);

        builder.HasIndex(si => si.ProductId);

        // Relationships
        builder
            .HasOne(si => si.Subscription)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SubscriptionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne(si => si.Product)
            .WithMany(p => p.SubscriptionItems)
            .HasForeignKey(si => si.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
