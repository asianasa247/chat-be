using ManageEmployee.Entities.UserEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManageEmployee.Dal.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
           .HasMany(p => p.UserToContracts)
           .WithOne()
           .HasForeignKey(p => p.UserId);
    }
}
