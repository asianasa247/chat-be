using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductModel
{
    public int Id { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public DateTime Date { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    public string? ProcedureStatusName { get; set; }
    public List<PlanningProduceProductDetailModel>? Items { get; set; }
    public List<BillPromotionModel>? BillPromotions { get; set; }
    public ProcedureForCreatePlanningProduct ProcedureCode { get; set; }
}
