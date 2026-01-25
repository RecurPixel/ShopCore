using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ImageUrl).IsRequired().HasMaxLength(500);

        builder.Property(pi => pi.ThumbnailUrl).HasMaxLength(500);

        builder.Property(pi => pi.AltText).HasMaxLength(200);

        // Indexes
        builder.HasIndex(pi => pi.ProductId);
    }
}
