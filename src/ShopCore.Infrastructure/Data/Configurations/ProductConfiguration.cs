using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        builder.Property(p => p.Slug).IsRequired().HasMaxLength(200);

        builder.HasIndex(p => p.Slug).IsUnique();

        builder.Property(p => p.Description).HasMaxLength(2000);

        builder.Property(p => p.ShortDescription).HasMaxLength(500);

        builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);

        builder.Property(p => p.CompareAtPrice).HasPrecision(18, 2);

        builder.Property(p => p.CostPerItem).HasPrecision(18, 2);

        builder.Property(p => p.SKU).HasMaxLength(50);

        builder.HasIndex(p => p.SKU).IsUnique();

        builder.Property(p => p.Barcode).HasMaxLength(50);

        builder.Property(p => p.Weight).HasPrecision(10, 2);

        builder.Property(p => p.WeightUnit).HasMaxLength(10);

        builder.Property(p => p.Dimensions).HasMaxLength(50);

        builder.Property(p => p.SubscriptionDiscount).HasPrecision(5, 2);

        builder.Property(p => p.MetaTitle).HasMaxLength(200);

        builder.Property(p => p.MetaDescription).HasMaxLength(500);

        builder.Property(p => p.MetaKeywords).HasMaxLength(500);

        builder.Property(p => p.AverageRating).HasPrecision(3, 2);

        builder.Property(p => p.Status).HasConversion<int>();

        // Ignore computed properties
        builder.Ignore(p => p.DiscountPercentage);
        builder.Ignore(p => p.IsInStock);
        builder.Ignore(p => p.IsOnSale);

        // Indexes for performance
        builder.HasIndex(p => p.VendorId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => new { p.VendorId, p.Status });
        builder.HasIndex(p => new { p.CategoryId, p.Status });

        builder
            .HasMany(p => p.Images)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Specifications)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(p => p.Reviews)
            .WithOne(r => r.Product)
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
