using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Helpers;

public static class ApplicationHelper
{
    public static void DatabaseMigration(this WebApplication app, ConfigurationManager configuration)
    {
        using (var scope = app.Services.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var log = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();

            var connectionStringProvider = serviceProvider.GetRequiredService<IConnectionStringProvider>();

            var databaseNames = GetDatabaseNames(configuration);

            foreach (var databaseName in databaseNames) 
            {
                Task.Run(async () =>
                {
                    try
                    {
                        var connectionString = connectionStringProvider.GetConnectionString(databaseName);

                        var dbContextOptionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

                        connectionStringProvider.SetupDbContextOptionsBuilder(dbContextOptionBuilder, connectionString);

                        using (var dataContext = new ApplicationDbContext(dbContextOptionBuilder.Options))
                        {
                            await dataContext.Database.MigrateAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, $"Migrating {databaseName} getting error");
                    }
                });
            }
        }
    }

    private static List<string> GetDatabaseNames(ConfigurationManager configuration)
    {
        var databaseNames = configuration.GetConnectionString("databaseNames").Split(";").ToList();
        var currentDatabase = configuration.GetConnectionString("DbName");

        databaseNames.Add(currentDatabase);

        return databaseNames.Distinct().ToList();
    }
}
