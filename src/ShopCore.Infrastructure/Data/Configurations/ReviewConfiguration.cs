using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating).IsRequired();

        builder.Property(r => r.Title).HasMaxLength(200);

        builder.Property(r => r.Comment).IsRequired().HasMaxLength(2000);

        builder.Property(r => r.ImageUrls).HasMaxLength(2000);

        builder.Property(r => r.VendorResponse).HasMaxLength(1000);

        // Indexes
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => new { r.ProductId, r.UserId }).IsUnique();

        // Relationships
        builder
            .HasOne(r => r.OrderItem)
            .WithMany()
            .HasForeignKey(r => r.OrderItemId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
