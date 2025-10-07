using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ConfigurationEntities;
using ManageEmployee.Services.Interfaces.Configurations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ManageEmployee.Services;

public class ConfigurationService: IConfigurationService
{
    private readonly ApplicationDbContext _context;

    public ConfigurationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<T> GetAsync<T>(string key, int userId)
    {
        var configuration = await GetByKeyAsync(key, userId);
        if (configuration == null)
        {
            return default;
        }
        var jsonObject = JsonConvert.DeserializeObject<T>(configuration.Data ?? string.Empty);
        return jsonObject;
    }

    public async Task SetAsync(string key, object data, int userId)
    {
        var configuarationUser = await _context.ConfigurationUsers.FirstOrDefaultAsync(x => x.Key == key && x.UserId == userId);

        if (configuarationUser is null)
        {
            configuarationUser = new ConfigurationUser();
            configuarationUser.Key = key;
            configuarationUser.UserId = userId;


            _context.ConfigurationUsers.Add(configuarationUser);

        }
        else
        {
            _context.ConfigurationUsers.Update(configuarationUser);

        }

        configuarationUser.Data = JsonConvert.SerializeObject(data);

        await _context.SaveChangesAsync();
    }

    private async Task<Configuration> GetByKeyAsync(string key, int userId)
    {
        var configuarationUser = await _context.ConfigurationUsers.FirstOrDefaultAsync(x => x.Key == key && x.UserId == userId);
        if (configuarationUser == null)
        {
            return await _context.Configurations.FirstOrDefaultAsync(x => x.Key == key);
        }

        return new Configuration
        {
            Data = configuarationUser.Data,
            Key = configuarationUser.Key
        };
    }
}