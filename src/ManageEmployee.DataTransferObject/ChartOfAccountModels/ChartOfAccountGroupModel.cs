namespace ManageEmployee.DataTransferObject.ChartOfAccountModels;

public class ChartOfAccountGroupModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public List<string> Details { get; set; } = new List<string>();
}