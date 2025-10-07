namespace ManageEmployee.Entities.ProcedureEntities;

public class P_Kpi_Item
{
    public int Id { get; set; }
    public int P_KpiId { get; set; }
    public int? UserId { get; set; }
    public double? Point { get; set; }// nhap tren quy trinh
}
