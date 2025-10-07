using ManageEmployee.Entities.SalaryEntities;

namespace ManageEmployee.Services.Interfaces.Users.Salaries;

public interface ISalaryLevelService
{
    IEnumerable<SalaryLevel> GetAll(int currentPage, int pageSize, string keyword);

    SalaryLevel Create(SalaryLevel param);

    void Update(SalaryLevel param);

    void Delete(int id);

    int Count(string keyword);
}
