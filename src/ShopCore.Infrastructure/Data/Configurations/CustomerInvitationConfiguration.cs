using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class CustomerInvitationConfiguration : IEntityTypeConfiguration<CustomerInvitation>
{
    public void Configure(EntityTypeBuilder<CustomerInvitation> builder)
    {
        builder.ToTable("CustomerInvitations");

        builder.HasKey(ci => ci.Id);

        builder.Property(ci => ci.InvitationToken)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(ci => ci.InvitationToken)
            .IsUnique();

        builder.Property(ci => ci.CustomerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ci => ci.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15);

        builder.Property(ci => ci.Email)
            .HasMaxLength(256);

        builder.Property(ci => ci.DeliveryAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(ci => ci.Pincode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ci => ci.SubscriptionItemsJson)
            .IsRequired();

        builder.Property(ci => ci.Frequency)
            .HasConversion<int>();

        builder.Property(ci => ci.PreferredDeliveryTime)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(ci => ci.DepositAmount)
            .HasPrecision(18, 2);

        builder.Property(ci => ci.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(ci => ci.Vendor)
            .WithMany(v => v.CustomerInvitations)
            .HasForeignKey(ci => ci.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ci => ci.InvitedUser)
            .WithMany(u => u.ReceivedInvitations)
            .HasForeignKey(ci => ci.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(ci => ci.VendorId);
        builder.HasIndex(ci => ci.Status);
        builder.HasIndex(ci => ci.ExpiresAt);
        builder.HasIndex(ci => ci.PhoneNumber);

        // Global query filter for soft delete
        builder.HasQueryFilter(ci => !ci.IsDeleted);
    }
}
