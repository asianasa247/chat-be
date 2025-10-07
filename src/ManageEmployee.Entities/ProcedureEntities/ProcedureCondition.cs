namespace ManageEmployee.Entities.ProcedureEntities;

public class ProcedureCondition
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string ProcedureCodes { get; set; }
    public bool IsDeleted { get; set; }

    public virtual P_Procedure? Procedure { get; set; }
}