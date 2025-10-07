namespace ManageEmployee.Services.Interfaces.Configurations;

public interface IConfigurationService
{
    Task<T> GetAsync<T>(string key, int userId);
    Task SetAsync(string key, object data, int userId);
}
