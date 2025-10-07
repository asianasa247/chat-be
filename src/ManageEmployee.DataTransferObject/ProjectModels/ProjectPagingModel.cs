namespace ManageEmployee.DataTransferObject.ProjectModels;

public class ProjectPagingModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool Status { get; set; }
    public bool AllowDelete { get; set; }
}
