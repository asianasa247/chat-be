using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureStatusService
{
    Task<IEnumerable<P_ProcedureStatus>> GetStatus(int procedureId);
    Task<IEnumerable<P_ProcedureStatus>> GetStatusForFilter(string pocedureCode);
}
