using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface ITillManagerService
{
    Task<PagingResult<TillManager>> GetAll(int pageIndex, int pageSize, string keyword);
    Task<TillManager> Create(TillManager request);
    Task<TillManager> GetById(int id);
    Task<string> Update(TillManager request);
    Task<double> CaculateAmountInTill(int userId);
    Task<TillManager> GetCurrentTillManager(int userId);
}
