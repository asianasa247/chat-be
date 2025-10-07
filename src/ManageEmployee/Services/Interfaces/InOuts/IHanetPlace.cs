using ManageEmployee.DataTransferObject.SelectModels;

namespace ManageEmployee.Services.Interfaces.InOuts
{
    public interface IHanetPlace
    {
        Task<List<SelectListModel>> GetList();
    }
}
