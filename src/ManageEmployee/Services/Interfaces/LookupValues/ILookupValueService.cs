using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.LookupValues;

public interface ILookupValueService
{
    Task<List<LookupValue>> GetLookupValues(string scope);
}
