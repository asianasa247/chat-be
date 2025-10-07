using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Jobs;

public interface IJobService
{
    IEnumerable<Job> GetAll();

    IEnumerable<Job> GetPaging(int currentPage, int pageSize, string keyword);

    Job GetById(int id);

    Task<string> Create(Job param);

    Task<string> Update(Job param);

    int Count(string keyword);

    Task<object> GetJobsAndStatusesExistingCustomerHistoriesAsync(int customerId);

    string Delete(int id);
}
