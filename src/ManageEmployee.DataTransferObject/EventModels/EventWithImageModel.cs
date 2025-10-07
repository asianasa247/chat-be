using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.EventModels;
public class EventWithImageModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Order { get; set; }
    public List<IFormFile>? Files { get; set; }
    public string? FileStored { get; set; }
    public DateTime Date { get; set; }
    public string? LinkDriver { get; set; }
    public string? Note { get; set; }
}
