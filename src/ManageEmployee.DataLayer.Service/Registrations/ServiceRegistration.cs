using ManageEmployee.DataLayer.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ManageEmployee.DataLayer.Service.Registrations;

public static class ServiceRegistration
{
    public static void Register(IServiceCollection services)
    {
        services.AddTransient<IContractorService, ContractorService>();
    }
}