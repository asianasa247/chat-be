using ManageEmployee.Entities.ProcedureEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class ProcedureConditionConfiguration : IEntityTypeConfiguration<ProcedureCondition>
{
    public void Configure(EntityTypeBuilder<ProcedureCondition> builder)
    {
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => new { p.Code, p.ProcedureCodes }).IsUnique();

        builder.Property(p => p.Name)
            .HasColumnName("Name")
            .HasMaxLength(256);
        builder.Property(p => p.Code)
            .HasColumnName("Code")
            .HasMaxLength(256);
        builder.Property(p => p.ProcedureCodes)
            .HasColumnName("ProcedureCodes")
            .HasMaxLength(256);

        builder.Property(p => p.IsDeleted).HasDefaultValue(false);

        builder.HasOne(p => p.Procedure)
            .WithMany(p => p.ProcedureConditions)
            .HasPrincipalKey(p => p.Code);
    }
}
