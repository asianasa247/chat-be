using ManageEmployee.Entities.CategoryEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class CategoryStatusWebPeriodGoodConfiguration : IEntityTypeConfiguration<CategoryStatusWebPeriodGood>
{
    public void Configure(EntityTypeBuilder<CategoryStatusWebPeriodGood> builder)
    {
        builder
            .HasOne(d => d.CategoryStatusWebPeriod)
            .WithMany(p => p.CategoryStatusWebPeriodGoods)
            .HasForeignKey(d => d.CategoryStatusWebPeriodId);

        builder
            .HasOne(d => d.Goods)
            .WithMany(p => p.CategoryStatusWebPeriodGoods)
            .HasForeignKey(d => d.GoodId);
    }
}
