using ManageEmployee.Entities.ProcedureEntities;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureConditionService
{
    Task<IEnumerable<ProcedureCondition>> GetList(string procedureCode);
}
