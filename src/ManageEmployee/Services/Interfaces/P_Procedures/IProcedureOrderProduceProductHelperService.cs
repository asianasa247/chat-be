using ManageEmployee.DataTransferObject.P_ProcedureView;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureOrderProduceProductHelperService
{
    Task<ProcedureStatusModelResponse> GetStatusAcceptForOrderProduceProduct(int P_ProcedureStatusId, int orderProduceProductId, int userId, int year);
}
