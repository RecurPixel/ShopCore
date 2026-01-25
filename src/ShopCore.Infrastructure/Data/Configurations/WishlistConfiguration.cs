using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class WishlistConfiguration : IEntityTypeConfiguration<Wishlist>
{
    public void Configure(EntityTypeBuilder<Wishlist> builder)
    {
        builder.HasKey(w => w.Id);

        // Indexes
        builder.HasIndex(w => w.UserId);

        builder.HasIndex(w => new { w.UserId, w.ProductId }).IsUnique();

        // Relationships already defined in User and Product configurations
    }
}
