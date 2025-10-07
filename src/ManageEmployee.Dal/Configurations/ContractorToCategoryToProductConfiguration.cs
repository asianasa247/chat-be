using ManageEmployee.Entities.ContractorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class ContractorToCategoryToProductConfiguration : IEntityTypeConfiguration<ContractorToCategoryToProduct>
{
    public void Configure(EntityTypeBuilder<ContractorToCategoryToProduct> builder)
    {
        builder.HasKey(p => p.ContractorToCategoryToProductId);

        builder.HasIndex(p => new
        {
            p.ContractToCategoryId,
            p.ProductId,
        });

        builder.Property(p => p.ContractorToCategoryToProductId)
            .HasColumnName("contractor_to_category_to_productId");

        builder.Property(p => p.ProductId)
            .HasColumnName("productId");

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(p => p.ContractToCategoryId);

        builder.ToTable("ContractorToCategoryToProduct");
    }
}