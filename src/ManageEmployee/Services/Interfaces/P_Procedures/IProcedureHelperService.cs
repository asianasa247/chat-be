using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureHelperService
{
    Task<bool> CheckPermissionAddProcedure(string procedureCode, int userId);
    Task DeleteLog(int procedureId, string procedureCode);
    Task<bool> ExistLog(int procedureId, string procedureCode);
    string GetCode(string codeNumber, string prefix);
    Task<IEnumerable<int>> GetLogStep(string procedureCode, int procedureId);
    IQueryable<ProcedureLog> GetProcedureLog(string procedureCode, int procedureId);
    string GetProcedureNumber(string procedureNumber, int length = 7);
    Task<IEnumerable<int?>> GetProcedureStatusIds(int userId, ProduceProductStatusTab statusTab, string produceCode);

    Task<ProcedureStatusModelResponse> GetStatusAccept(int? procedureStatusId, int userId, bool isCheckCondition = false);

    Task<ProcedureStatusModelResponse> GetStatusInit(string procedureCode);
    Task<IEnumerable<int>> GetUserFinish(int? procedureStatusId);
    Task WriteProcedureLog(string procedureCode, int procedureStatusId, int userId, int procedureId, string procedureName, bool isNotAccept = false);
    Task WriteProcedureLogToSendNotification(string procedureCode, int procedureToStatusId, int procedureId, int userId, string procedureName, bool isFinished = false);
}
