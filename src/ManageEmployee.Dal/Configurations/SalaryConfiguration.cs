using ManageEmployee.Entities.SalaryEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class SalaryConfiguration : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder
            .HasKey(e => new { e.Userid, e.Month, e.Year });
    }
}
