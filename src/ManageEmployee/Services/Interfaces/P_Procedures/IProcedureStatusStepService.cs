using ManageEmployee.DataTransferObject.P_ProcedureView;

namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureStatusStepService
{
    Task<IEnumerable<P_ProcedureStatusStepModel>> Getter(int procedureId);
    Task Setter(int procedureId, List<P_ProcedureStatusStepModel> form);
}
