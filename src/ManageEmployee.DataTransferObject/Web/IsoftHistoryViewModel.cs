namespace ManageEmployee.DataTransferObject.Web;

public class IsoftHistoryViewModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? ClassName { get; set; }
    public string? Content { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? Order { get; set; }
}
