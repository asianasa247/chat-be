namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_Leave_ItemModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsLicensedItem { get; set; }
    public string SymbolCode { get; set; }//P; K
}
