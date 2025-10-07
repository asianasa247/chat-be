using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;

public class GoodsQuotaPagingGetterModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? ItemNames { get; set; }
    public string? ItemCodes { get; set; }
    public string? GoodsQuotaRecipeName { get; set; }
    public int GoodsQuotaRecipeId { get; set; }
    public DateTime Date { get; set; }
    public bool IsFinished { get; set; }
    public string ProcedureNumber { get; set; }
    public string ProcedureStatusName { get; set; }
    public bool ShoulNotAccept { get; set; }
}
