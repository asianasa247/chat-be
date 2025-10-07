using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.CarModels;

public class CarFieldSetupGetterModel
{
    public int Id { get; set; }
    public int CarFieldId { get; set; }
    public string? CarFieldName { get; set; }
    public double? ValueNumber { get; set; }
    public DateTime? ValueDate { get; set; }
    public DateTime? FromAt { get; set; }
    public DateTime? ToAt { get; set; }
    public DateTime? WarningAt { get; set; }
    public List<int>? UserIds { get; set; }
    public string? Note { get; set; }
    public FileDetailModel? File { get; set; }

}
