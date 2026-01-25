using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class SubscriptionInvoiceConfiguration : IEntityTypeConfiguration<SubscriptionInvoice>
{
    public void Configure(EntityTypeBuilder<SubscriptionInvoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        builder.Property(i => i.Subtotal).HasPrecision(18, 2);

        builder.Property(i => i.Tax).HasPrecision(18, 2);

        builder.Property(i => i.Total).HasPrecision(18, 2);

        builder.Property(i => i.PaidAmount).HasPrecision(18, 2);

        builder.Property(i => i.Status).HasConversion<int>();

        builder.Property(i => i.PaymentMethod).HasMaxLength(50);

        builder.Property(i => i.PaymentTransactionId).HasMaxLength(100);

        // Ignore computed properties
        builder.Ignore(i => i.BalanceDue);
        builder.Ignore(i => i.IsFullyPaid);
        builder.Ignore(i => i.IsOverdue);

        // Indexes
        builder.HasIndex(i => i.SubscriptionId);
        builder.HasIndex(i => i.Status);
        builder.HasIndex(i => i.DueDate);
        builder.HasIndex(i => new { i.UserId, i.Status });

        // Relationships
        builder
            .HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(i => i.Vendor)
            .WithMany()
            .HasForeignKey(i => i.VendorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
