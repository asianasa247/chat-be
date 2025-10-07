using ManageEmployee.Entities.ContractorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class ContractorToCategoryConfiguration : IEntityTypeConfiguration<ContractorToCategory>
{
    public void Configure(EntityTypeBuilder<ContractorToCategory> builder)
    {
        builder.HasKey(p => p.ContractorToCategoryId);

        builder.HasIndex(p => new
        {
            p.CategoryName,
            p.UserToContractorId,
            p.SortOrder,
        });

        builder.Property(p => p.ContractorToCategoryId)
            .HasColumnName("contractor_to_categoryId");

        builder.Property(p => p.UserToContractorId)
            .HasColumnName("user_to_contractorId");

        builder.Property(p => p.CategoryName)
            .HasColumnName("category_name");

        builder.Property(p => p.SortOrder)
            .HasColumnName("sort_order");

        builder.Property(p => p.IsDeleted)
            .HasColumnName("isDeleted");

        builder.HasMany(p => p.ContractorToCategoryToProducts)
            .WithOne()
            .HasForeignKey(p => p.ContractToCategoryId);

        builder.ToTable("ContractorToCategory");
    }
}