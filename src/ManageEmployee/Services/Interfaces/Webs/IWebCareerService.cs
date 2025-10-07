using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;

namespace ManageEmployee.Services.Interfaces.Webs;

public interface IWebCareerService
{
    IEnumerable<CareerViewModel> GetAll();

    Task<PagingResult<CareerViewModel>> SearchCareer(CareerPagingRequestModel searchRequest);

    CareerViewModel Create(CareerViewModel request);

    CareerViewModel GetById(int id);

    CareerViewModel Update(CareerViewModel request);

    void Delete(int id);
}
