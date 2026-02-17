using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class VendorPayoutConfiguration : IEntityTypeConfiguration<VendorPayout>
{
    public void Configure(EntityTypeBuilder<VendorPayout> builder)
    {
        builder.HasKey(vp => vp.Id);

        builder.Property(vp => vp.PayoutNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(vp => vp.PayoutNumber).IsUnique();

        builder.Property(vp => vp.TotalSales).HasPrecision(18, 2);

        builder.Property(vp => vp.CommissionAmount).HasPrecision(18, 2);

        builder.Property(vp => vp.DeductionsAmount).HasPrecision(18, 2);

        builder.Property(vp => vp.NetPayout).HasPrecision(18, 2);

        builder.Property(vp => vp.Status).HasConversion<int>();

        builder.Property(vp => vp.PayoutMethod).HasConversion<string>().HasMaxLength(50);

        builder.Property(vp => vp.PayoutTransactionId).HasMaxLength(100);

        builder.Property(vp => vp.TransactionReference).HasMaxLength(100);

        // Ignore computed property
        builder.Ignore(vp => vp.CalculatedNetPayout);

        // Indexes
        builder.HasIndex(vp => vp.VendorId);

        builder.HasIndex(vp => vp.Status);

        // Relationships
        builder
            .HasOne(vp => vp.Vendor)
            .WithMany(v => v.Payouts)
            .HasForeignKey(vp => vp.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(vp => vp.ProcessedByUser)
            .WithMany()
            .HasForeignKey(vp => vp.ProcessedBy)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
