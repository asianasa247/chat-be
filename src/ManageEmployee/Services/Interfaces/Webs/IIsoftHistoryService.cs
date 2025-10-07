using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Webs;

public interface IIsoftHistoryService
{
    PagingResult<IsoftHistory> GetAll(int pageIndex, int pageSize, string keyword);
    Task<string> Create(IsoftHistory request);
    IsoftHistory GetById(int id);
    Task<string> Update(IsoftHistory request);
    void Delete(int id);
    HistoryByClassNameModel GetHistoryByClassName(string className, string keyword);
}
