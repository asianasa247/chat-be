namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerProcedureProductExporter
{
    Task<string> Export(int id);
}
