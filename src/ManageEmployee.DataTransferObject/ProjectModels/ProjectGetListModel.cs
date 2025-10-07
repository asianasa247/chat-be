namespace ManageEmployee.DataTransferObject.ProjectModels;

public class ProjectGetListModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool Status { get; set; }
    public bool AllowDelete { get; set; }
    public bool AllowUpdateCode { get; set; }
}
