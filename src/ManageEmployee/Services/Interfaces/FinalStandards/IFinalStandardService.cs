using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.FinalStandards;

public interface IFinalStandardService
{
    Task<PagingResult<FinalStandard>> GetPaging(PagingRequestModel param);

    IEnumerable<FinalStandard> GetAll();

    FinalStandard Create(FinalStandard param);

    void Update(FinalStandard param);

    Task Delete(int id);

    FinalStandard GetById(int Id);

    Task<List<FinalStandardDetailModel>> GetFinalStandardDetail(PagingationFinalStandardRequestModel filter, int year);
    Task SetIntoLedger(List<FinalStandardDetailModel> finalStandards, int isInternal, int year);
}
