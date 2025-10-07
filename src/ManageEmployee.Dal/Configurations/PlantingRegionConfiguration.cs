using ManageEmployee.Entities.Cultivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations
{
    internal class PlantingRegionConfiguration : IEntityTypeConfiguration<PlantingRegion>
    {
        public void Configure(EntityTypeBuilder<PlantingRegion> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code).HasMaxLength(64).IsRequired();
            builder.Property(p => p.Name).HasMaxLength(256).IsRequired();
            builder.Property(p => p.Note).HasMaxLength(1024);
            builder.Property(p => p.Address).HasMaxLength(512);
            builder.Property(p => p.IssuerUnitCode).HasMaxLength(64);

            builder.HasIndex(p => new { p.CountryId, p.Code }).IsUnique();

            // KHÔNG cascade về PlantingTypes để tránh multiple cascade paths
            builder.HasOne(p => p.Type)
                   .WithMany()
                   .HasForeignKey(p => p.TypeId)
                   .OnDelete(DeleteBehavior.Restrict); // hoặc .NoAction()
        }
    }
}
