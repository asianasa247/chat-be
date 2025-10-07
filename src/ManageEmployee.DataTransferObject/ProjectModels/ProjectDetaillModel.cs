namespace ManageEmployee.DataTransferObject.ProjectModels;

public class ProjectDetaillModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool Status { get; set; }
    public bool AllowUpdateCode { get; set; }
}
