using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.HasKey(osh => osh.Id);

        builder.Property(osh => osh.Status).HasConversion<int>();

        builder.Property(osh => osh.Notes).HasMaxLength(500);

        builder.Property(osh => osh.ChangedAt).HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(osh => osh.OrderId);
    }
}
