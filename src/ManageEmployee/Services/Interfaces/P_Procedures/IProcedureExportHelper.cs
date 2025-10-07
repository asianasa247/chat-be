namespace ManageEmployee.Services.Interfaces.P_Procedures;

public interface IProcedureExportHelper
{
    Task<string> SignPlace(int id, string procedureCode);
    Task<string> SignPlaceLastest(int id, string procedureCode);
    Task<string> SignPlaceLastestOrder(int id, string procedureCode);
    Task<string> SignPlaceOrder(int id, string procedureCode, bool IsMaxcolumn = false);
    Task<string> SignPlaceSameTr(int id, string procedureCode);
}
