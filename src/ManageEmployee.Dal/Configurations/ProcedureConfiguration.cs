using ManageEmployee.Entities.ProcedureEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

public class ProcedureConfiguration : IEntityTypeConfiguration<P_Procedure>
{
    public void Configure(EntityTypeBuilder<P_Procedure> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Code);

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasMaxLength(256);
        builder.Property(p => p.Code)
            .HasColumnName("Code")
            .HasMaxLength(256);
        builder.Property(p => p.Note)
            .HasColumnName("Note")
            .HasMaxLength(256);

        builder.HasMany(p => p.ProcedureConditions)
            .WithOne(p => p.Procedure)
            .HasPrincipalKey(p => p.Code);
    }
}
