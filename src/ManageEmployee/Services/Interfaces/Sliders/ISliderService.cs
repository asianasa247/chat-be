using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Sliders;

public interface ISliderService
{
    Task<IEnumerable<SliderModel>> GetAll();
    Task<PagingResult<SliderModel>> GetAll(SlideRequestModel param);
    Task<string> Create(SliderModel request);
    Task<SliderModel> GetById(int id);
    Task<string> Update(SliderModel request);
    Task Delete(int id);
}
