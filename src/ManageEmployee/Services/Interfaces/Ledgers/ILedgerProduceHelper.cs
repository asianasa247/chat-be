namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerProduceHelper
{
    Task<string> SignPlace(string orginalCompanyName, int id, string procedureCode);
}
