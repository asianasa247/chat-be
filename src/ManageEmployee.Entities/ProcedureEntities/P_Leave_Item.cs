namespace ManageEmployee.Entities.ProcedureEntities;

public class P_Leave_Item
{
    public int Id { get; set; }
    public int LeaveId { get; set; }
    public DateTime Date { get; set; }
    public bool IsLicensedItem { get; set; }
    public string SymbolCode {  get; set; }//P; K
}
