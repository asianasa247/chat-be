using ManageEmployee.DataTransferObject.ConvertProductModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ConvertProductEntities;

namespace ManageEmployee.Services.Interfaces.ConvertToProduct
{
    public interface IConvertProductService
    {
        Task<ConvertProduct> Add(DTOConvert model);
        Task<ConvertProduct> Update(int id, DTOConvert model);
        Task<bool> Delete(int id);
        Task<ConvertProduct> GetById(int id);
        Task<object> GetAll(PagingRequestModel request);
    }

}
