using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(50);

        builder.HasIndex(c => c.Code).IsUnique();

        builder.Property(c => c.Description).HasMaxLength(500);

        builder.Property(c => c.Type).HasConversion<int>();

        builder.Property(c => c.DiscountPercentage).HasPrecision(5, 2);

        builder.Property(c => c.DiscountAmount).HasPrecision(18, 2);

        builder.Property(c => c.MinOrderValue).HasPrecision(18, 2);

        builder.Property(c => c.MaxDiscount).HasPrecision(18, 2);

        // Ignore computed property
        builder.Ignore(c => c.IsValid);

        // Indexes
        builder.HasIndex(c => c.IsActive);

        builder.HasIndex(c => new { c.ValidFrom, c.ValidUntil });

        // Relationships
        builder
            .HasMany(c => c.Orders)
            .WithOne(o => o.Coupon)
            .HasForeignKey(o => o.CouponId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
