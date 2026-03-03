using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> builder)
    {
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Channel).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Provider).HasMaxLength(100);
        builder.Property(l => l.Recipient).IsRequired().HasMaxLength(255);
        builder.Property(l => l.Status).IsRequired().HasMaxLength(20);
        builder.Property(l => l.ProviderId).HasMaxLength(200);
        builder.Property(l => l.Error).HasMaxLength(1000);

        builder.HasIndex(l => l.SentAt);
        builder.HasIndex(l => l.Status);
    }
}
