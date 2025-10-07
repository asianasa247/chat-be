using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.UserModels.SalaryModels;

namespace ManageEmployee.Services.Interfaces.Users.Salaries;

public interface ISalaryUserVersionService
{
    Task Delete(int id);

    Task<string> ExportExcel();

    Task<SalaryUserVersionDetailModel> GetDetail(int id);

    Task<PagingResult<SalaryUserVersionModel>> GetPaging(PagingRequestModel param);

    Task ImportExcel(IFormFile file);

    Task SetData(SalaryUserVersionUpdateModel param, int userId);
}
