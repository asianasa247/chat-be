using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Targets;

public interface ITargetService
{
    IEnumerable<Target> GetAll();

    IEnumerable<Target> GetAll(int currentPage, int pageSize, string keyword);

    Target GetById(int id);

    Target Create(Target param);

    void Update(Target param);

    void Delete(int id);
}
