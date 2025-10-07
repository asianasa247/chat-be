namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_SalaryAdvance_ItemForUser
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public double Value { get; set; }
    public string? Note { get; set; }
}
