using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;
using CartEntity = ShopCore.Domain.Entities.Cart;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);

        builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);

        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PhoneNumber).IsRequired().HasMaxLength(15);

        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.Property(u => u.PasswordHash).IsRequired();

        builder.Property(u => u.Role).HasConversion<int>();

        builder.Property(u => u.EmailVerificationToken).HasMaxLength(100);

        builder.Property(u => u.PasswordResetToken).HasMaxLength(100);

        builder.Property(u => u.RefreshToken).HasMaxLength(500);

        builder.Property(u => u.AvatarUrl).HasMaxLength(500);

        // Ignore computed property
        builder.Ignore(u => u.FullName);

        // Relationships
        builder
            .HasOne(u => u.VendorProfile)
            .WithOne(v => v.User)
            .HasForeignKey<VendorProfile>(v => v.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.Addresses)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.Orders)
            .WithOne(o => o.User)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(u => u.Cart)
            .WithOne(c => c.User)
            .HasForeignKey<CartEntity>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(u => u.Wishlists)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(u => u.Subscriptions)
            .WithOne(s => s.User)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
