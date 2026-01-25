using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class VendorProfileConfiguration : IEntityTypeConfiguration<VendorProfile>
{
    public void Configure(EntityTypeBuilder<VendorProfile> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.BusinessName).IsRequired().HasMaxLength(200);

        builder.Property(v => v.BusinessDescription).HasMaxLength(1000);

        builder.Property(v => v.BusinessLogo).HasMaxLength(500);

        builder.Property(v => v.BusinessAddress).IsRequired().HasMaxLength(500);

        builder.Property(v => v.GstNumber).IsRequired().HasMaxLength(20);

        builder.HasIndex(v => v.GstNumber).IsUnique();

        builder.Property(v => v.PanNumber).IsRequired().HasMaxLength(10);

        builder.Property(v => v.BankName).IsRequired().HasMaxLength(100);

        builder.Property(v => v.BankAccountNumber).IsRequired().HasMaxLength(50);

        builder.Property(v => v.BankIfscCode).IsRequired().HasMaxLength(15);

        builder.Property(v => v.BankAccountHolderName).IsRequired().HasMaxLength(100);

        builder.Property(v => v.CommissionRate).HasPrecision(5, 2).HasDefaultValue(5.00m);

        builder.Property(v => v.Status).HasConversion<int>();

        builder.Property(v => v.DefaultDepositAmount).HasPrecision(18, 2);

        builder.Property(v => v.AverageRating).HasPrecision(3, 2);

        builder.Property(v => v.TotalRevenue).HasPrecision(18, 2);

        // Indexes
        builder.HasIndex(v => v.UserId).IsUnique();

        // Relationships
        builder
            .HasMany(v => v.Products)
            .WithOne(p => p.Vendor)
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasMany(v => v.Payouts)
            .WithOne(s => s.Vendor)
            .HasForeignKey(s => s.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
