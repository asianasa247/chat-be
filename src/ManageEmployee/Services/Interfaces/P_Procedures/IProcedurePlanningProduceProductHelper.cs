using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Entities.ProduceProductEntities;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedurePlanningProduceProductHelper
{
    Task<ProcedureStatusModelResponse> GetStatusAcceptForPlanningProduceProduct(int procedureStatusId, int planningProduceProductId, int userId, int year);
    Task<bool> SendToCashierAsync(IEnumerable<PlanningProduceProductDetail> planningProduceProductDetails, int planningProduceProductId, int userId, string billStatus, int year, string produceCode);
}
