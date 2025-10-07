using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.AdditionWebServices;

public abstract class AdditionWebServiceBase
{
    protected readonly IDbContextFactory _dbContextFactory;
    protected AdditionWebServiceBase(IDbContextFactory dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected ApplicationDbContext GetApplicationDbContext(AdditionWeb additionWeb)
    {
        var hasConnectionString = !string.IsNullOrWhiteSpace(additionWeb.ConnectionString);

        if (!hasConnectionString)
        {
            return _dbContextFactory.GetDbContextFromDatabaseName(additionWeb.DbName);
        }

        if(additionWeb.ConnectionString.Contains("{dbName}") && hasConnectionString)
        {
            return _dbContextFactory.GetDbContextFromDatabaseName(
                additionWeb.DbName, 
                additionWeb.ConnectionString
            );
        }

        return _dbContextFactory.GetDbContext(additionWeb.ConnectionString);
    }
}
