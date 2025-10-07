using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
public class GoodsQuotaModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public DateTime Date { get; set; }
    public bool IsFinished { get; set; }
    public string? ProcedureNumber { get; set; }
    public int? ProcedureStatusId { get; set; }
    [Required]
    public int GoodsQuotaRecipeId { get; set; }
    public List<GoodsQuotaDetailModel>? Items { get; set; }
}
