using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.CarModels;

public class PetrolConsumptionReportModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int Month { get; set; }
    public string? ExplainNote { get; set; }
    public double AdvanceAmount { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal ExpenseAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public List<FileDetailModel>? Files { get; set; }
    public string? UserName { get; set; }
    public double PetroPrice { get; set; }
    public double KmFrom { get; set; }
    public double KmTo { get; set; }
}
