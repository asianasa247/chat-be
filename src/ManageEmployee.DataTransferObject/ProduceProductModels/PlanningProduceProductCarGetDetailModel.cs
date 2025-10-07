using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductCarGetDetailModel
{
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public string? Note { get; set; }
    public List<FileDetailModel>? FileLinks { get; set; }
    public string? LicensePlates { get; set; }
    public bool ShouldExport { get; set; }
    public bool IsCanceled {  get; set; }
    public List<PlanningProduceProductGoodGetDetailModel>? Goods { get; set; }
}
