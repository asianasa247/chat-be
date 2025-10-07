using ManageEmployee.Handlers;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Descriptions;

public interface IDescriptionService
{
    IEnumerable<Description> GetAll();
    Task<List<Description>> GetPage(int currentPage, int pageSize, string query = "");
    Task<int> CountAll();
    Task<CustomActionResult<Description>> Create(Description entity);
    Task<CustomActionResult<Description>> Update(Description entity);
    Task<CustomActionResult<Description>> Delete(long id);
    string Delete(IEnumerable<long> ids);
}
