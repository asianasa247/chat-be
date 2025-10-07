using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Certificates;

public interface ICertificateService
{
    IEnumerable<Certificate> GetAll();

    IEnumerable<Certificate> GetAll(int currentPage, int pageSize, string keyword);

    Certificate GetById(int id);

    Certificate Create(Certificate param);

    Task Update(Certificate param);

    Task Delete(int id);

    int Count(string keyword);
}
