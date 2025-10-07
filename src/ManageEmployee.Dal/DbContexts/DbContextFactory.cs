using ManageEmployee.Dal.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ManageEmployee.Dal.DbContexts;

public class DbContextFactory : IDbContextFactory
{
    private readonly IConfiguration _configuration;
    public DbContextFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ApplicationDbContext GetDbContext(string connectionStr)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>();
        options.UseSqlServer(connectionStr);
        options.AddInterceptors(new UpdateYearInterceptor());
        return new ApplicationDbContext(options.Options);
    }

    public ApplicationDbContext GetDbContextFromDatabaseName(string dbName)
    {
        var connectionStringPlaceHolder = _configuration.GetConnectionString("ConnStr");
        var connectionStr = connectionStringPlaceHolder.Replace("{dbName}", dbName);

        return GetDbContext(connectionStr);
    }

    public ApplicationDbContext GetDbContextFromDatabaseName(string dbName, string connectionStringPlaceHolder)
    {
        var connectionStr = connectionStringPlaceHolder.Replace("{dbName}", dbName);

        return GetDbContext(connectionStr);
    }
}