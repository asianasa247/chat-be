namespace ManageEmployee.Dal.DbContexts;

public interface IDbContextFactory
{
    ApplicationDbContext GetDbContext(string connectionStr);
    ApplicationDbContext GetDbContextFromDatabaseName(string dbName);
    ApplicationDbContext GetDbContextFromDatabaseName(string dbName, string connectionStringPlaceHolder);
}