using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Degrees;

public interface IDegreeService
{
    IEnumerable<Degree> GetAll();

    IEnumerable<Degree> GetAll(int currentPage, int pageSize, string keyword);

    Degree GetById(int id);

    Degree Create(Degree param);

    void Update(Degree param);

    void Delete(int id);

    int Count(string keyword);
}
