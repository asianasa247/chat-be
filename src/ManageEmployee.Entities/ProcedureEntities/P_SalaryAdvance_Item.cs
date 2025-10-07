namespace ManageEmployee.Entities.ProcedureEntities;

public class P_SalaryAdvance_Item
{
    public int Id { get; set; }
    public int P_SalaryAdvanceId { get; set; }
    public int UserId { get; set; }
    public int BranchId { get; set; }
    public double Value { get; set; }
}
