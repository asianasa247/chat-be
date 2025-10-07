using ManageEmployee.Entities.ContractorEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class UserToContractorConfiguration : IEntityTypeConfiguration<UserToContractor>
{
    public void Configure(EntityTypeBuilder<UserToContractor> builder)
    {
        builder.HasKey(p => p.UserToContractorId);

        builder.HasIndex(p => new
        {
            p.UserId,
            p.Domain,
        });
       
        builder.Property(p => p.UserToContractorId)
            .HasColumnName("user_to_contractorId");

        builder.Property(p => p.UserId)
            .HasColumnName("userId");

        builder.Property(p => p.Domain)
            .HasColumnName("domain");

        builder.Property(p => p.IsDeleted)
            .HasColumnName("isDeleted");

        builder.HasMany(p => p.ContractorToCategories)
            .WithOne()
            .HasForeignKey(p => p.ContractorToCategoryId);

        builder.ToTable("UserToContractor");
    }
}