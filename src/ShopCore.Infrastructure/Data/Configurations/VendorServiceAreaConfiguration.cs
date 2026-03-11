using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;
using System.Text.Json;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class VendorServiceAreaConfiguration : IEntityTypeConfiguration<VendorServiceArea>
{
    public void Configure(EntityTypeBuilder<VendorServiceArea> builder)
    {
        builder.ToTable("VendorServiceAreas");

        builder.HasKey(vsa => vsa.Id);

        builder.Property(vsa => vsa.AreaName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vsa => vsa.City)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vsa => vsa.State)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(vsa => vsa.Pincodes)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>()
            );

        builder.Property(vsa => vsa.IsActive)
            .HasDefaultValue(true);

        // Relationships
        builder.HasOne(vsa => vsa.Vendor)
            .WithMany(v => v.ServiceAreas)
            .HasForeignKey(vsa => vsa.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(vsa => vsa.VendorId);
        builder.HasIndex(vsa => vsa.IsActive);

        // Global query filter for soft delete
        builder.HasQueryFilter(vsa => !vsa.IsDeleted);
    }
}
