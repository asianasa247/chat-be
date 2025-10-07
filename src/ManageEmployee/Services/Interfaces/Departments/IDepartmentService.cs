using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.AddressEntities;

namespace ManageEmployee.Services.Interfaces.Departments;

public interface IDepartmentService
{
    IEnumerable<Department> GetAll();

    Task<PagingResult<Department>> GetAll(DepartmentRequest request);

    Department GetById(int id);

    bool checkMemberHaveWarehouseCode(int? id, string code);

    bool checkMemberHaveWarehouse(int id);

    Department Create(Department param);

    Department Update(int id, Department param);

    void Delete(int id);

    Task<IEnumerable<Department>> GetListDepartmentForTask(int userId);
}
