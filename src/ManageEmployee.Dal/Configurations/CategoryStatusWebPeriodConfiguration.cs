using ManageEmployee.Entities.CategoryEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class CategoryStatusWebPeriodConfiguration : IEntityTypeConfiguration<CategoryStatusWebPeriod>
{
    public void Configure(EntityTypeBuilder<CategoryStatusWebPeriod> builder)
    {

        builder
            .HasOne(d => d.Category)
            .WithMany(p => p.CategoryStatusWebPeriods)
            .HasForeignKey(d => d.CategoryId);

    }
}
