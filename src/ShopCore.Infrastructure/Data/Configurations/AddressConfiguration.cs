using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FullName).IsRequired().HasMaxLength(100);

        builder.Property(a => a.PhoneNumber).IsRequired().HasMaxLength(15);

        builder.Property(a => a.AddressLine1).IsRequired().HasMaxLength(200);

        builder.Property(a => a.AddressLine2).HasMaxLength(200);

        builder.Property(a => a.City).IsRequired().HasMaxLength(100);

        builder.Property(a => a.State).IsRequired().HasMaxLength(100);

        builder.Property(a => a.PinCode).IsRequired().HasMaxLength(10);

        // Indexes
        builder.HasIndex(a => a.UserId);

        // Relationships
        builder
            .HasMany(a => a.Orders)
            .WithOne(o => o.ShippingAddress)
            .HasForeignKey(o => o.ShippingAddressId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(a => a.Subscriptions)
            .WithOne(s => s.DeliveryAddress)
            .HasForeignKey(s => s.DeliveryAddressId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
