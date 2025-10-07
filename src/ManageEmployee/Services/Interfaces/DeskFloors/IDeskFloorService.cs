using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.DeskFloors;

public interface IDeskFloorService
{
    Task<IEnumerable<DeskFloor>> GetAll();
    Task<DeskFLoorPagingResult> GetPaging(DeskFLoorPagingationRequestModel param);
    Task Create(DeskFloor param);
    Task<DeskFloor> GetById(int id);
    Task<DeskFloor> GetByCode(string code);
    Task Update(DeskFloor param);
    Task Delete(int id);
    void UpdateDeskChoose(int id, bool isChoose);
    Task<IEnumerable<DeskFloor>> GetListDeskFree();
    Task ResetDeskChoose();
    Task UpdateStatus(int id, int statusId);
}
