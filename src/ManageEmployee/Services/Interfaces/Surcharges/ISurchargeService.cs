using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Surcharges;

public interface ISurchargeService
{
    Task<PagingResult<Surcharge>> GetAll(int pageIndex, int pageSize, string keyword);
    Task<string> Create(Surcharge request);
    Task<Surcharge> GetById(int id);
    Task<string> Update(Surcharge request);
    Task<string> Delete(int id);
    Task<Surcharge> GetCurrent();
}
