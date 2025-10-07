using ManageEmployee.Entities.Cultivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations
{
    internal class PlantingTypeConfiguration : IEntityTypeConfiguration<PlantingType>
    {
        public void Configure(EntityTypeBuilder<PlantingType> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code).HasMaxLength(64).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(256).IsRequired();

            builder.HasIndex(p => new { p.Category, p.Code }).IsUnique();
        }
    }
}
