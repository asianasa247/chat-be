using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IP_InventoryService
{
    Task<PagingResult<P_InventoryViewModel>> GetAll(int pageIndex, int pageSize, string keyword);

    P_InventoryViewModel GetById(int id);

    Task<string> Create(P_InventoryViewModel param);

    Task<string> Update(P_InventoryViewModel param);

    Task<string> Delete(int id);

    string GetProcedureNumber();

    Task<string> Accept(int id, int userId);

    IEnumerable<P_Inventory_Item_ViewModel> GetListGood(string wareHouse, string account, string detail1, string detail2);

    string ExportInventoryById(int inventoryId);
}
