using ManageEmployee.Entities.Cultivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations
{
    internal class PlantingBedConfiguration : IEntityTypeConfiguration<PlantingBed>
    {
        public void Configure(EntityTypeBuilder<PlantingBed> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code).HasMaxLength(64).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(256).IsRequired();
            builder.Property(p => p.Note).HasMaxLength(1024);

            builder.HasIndex(p => new { p.RegionId, p.Code }).IsUnique();

            // Bed -> Region: CASCADE
            builder.HasOne(p => p.Region)
                   .WithMany()
                   .HasForeignKey(p => p.RegionId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Bed -> Type: RESTRICT (NO ACTION)
            builder.HasOne(p => p.Type)
                   .WithMany()
                   .HasForeignKey(p => p.TypeId)
                   .OnDelete(DeleteBehavior.Restrict); // hoặc .NoAction()
        }
    }
}
