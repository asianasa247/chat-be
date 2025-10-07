using ManageEmployee.Dal.Interceptors;
using ManageEmployee.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Configurations;

public class ConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public ConnectionStringProvider(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    public string GetConnectionString(string? databaseName = null)
    {
        var connectionStringPlaceHolder = _configuration.GetConnectionString("ConnStr");

        var dbName = string.IsNullOrEmpty(databaseName) 
            ? GetDbName()
            : databaseName;

        return connectionStringPlaceHolder.Replace("{dbName}", dbName);
    }

    public string GetDbName()
    {
        var dbNameFromConfiguration = _configuration.GetConnectionString("DbName");
        if (_configuration.GetConnectionString("isMultiDb") != "1")
        {
            return dbNameFromConfiguration;
        }

        try
        {
            var dbNameFromRequest = _httpContextAccessor.HttpContext?.Request?.Headers["dbName"].FirstOrDefault();

            return string.IsNullOrEmpty(dbNameFromRequest) 
                    ? dbNameFromConfiguration
                    : dbNameFromRequest;
        }
        catch
        {
            return dbNameFromConfiguration;
        }
    }

    public void SetupDbContextOptionsBuilder(DbContextOptionsBuilder? dbContextOptionBuilder, string connectionString)
    {
        dbContextOptionBuilder ??= new DbContextOptionsBuilder();

        dbContextOptionBuilder.UseSqlServer(connectionString);
        dbContextOptionBuilder.AddInterceptors(new UpdateYearInterceptor());
        dbContextOptionBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
