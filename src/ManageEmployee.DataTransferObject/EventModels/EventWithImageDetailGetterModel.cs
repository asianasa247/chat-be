using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.EventModels;

public class EventWithImageDetailGetterModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Order { get; set; }
    public DateTime Date { get; set; }
    public string? LinkDriver { get; set; }
    public string? Note { get; set; }
    public List<FileDetailModel>? Files { get; set; }

}
