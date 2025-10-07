namespace ManageEmployee.Services.Interfaces.P_Procedures.ExpenditurePlans;

public interface IExpenditurePlanExporter
{
    Task<string> Export(int expenditurePlanId);
    Task<string> ExportAdvanceSettlement(int expenditurePlanId);
}
