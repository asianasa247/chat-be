using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Interfaces;

public interface IConnectionStringProvider
{
    string GetConnectionString(string? databaseName = null);
    string GetDbName();
    void SetupDbContextOptionsBuilder(DbContextOptionsBuilder? dbContextOptionBuilder, string connectionString);
}
