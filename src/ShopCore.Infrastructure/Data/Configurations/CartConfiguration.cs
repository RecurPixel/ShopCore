using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;
using CartEntity = ShopCore.Domain.Entities.Cart;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        // Indexes
        builder.HasIndex(c => c.UserId).IsUnique();

        // Relationships
        builder
            .HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<CartEntity>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
