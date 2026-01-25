using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopCore.Domain.Entities;

namespace ShopCore.Infrastructure.EntityConfigurations;

public class ProductSpecificationConfiguration : IEntityTypeConfiguration<ProductSpecification>
{
    public void Configure(EntityTypeBuilder<ProductSpecification> builder)
    {
        builder.HasKey(ps => ps.Id);

        builder.Property(ps => ps.Name).IsRequired().HasMaxLength(100);

        builder.Property(ps => ps.Value).IsRequired().HasMaxLength(200);

        // Indexes
        builder.HasIndex(ps => ps.ProductId);
    }
}
