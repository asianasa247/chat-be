using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Inventorys;

public interface IInventoryService
{
    List<Inventory> GetListData(PagingRequestModel param, int year);
    string Create(List<Inventory> datas);
    IEnumerable<Inventory> GetListInventory(InventoryRequestModel param);
    List<DateTime> GetListDateInventory();
}
