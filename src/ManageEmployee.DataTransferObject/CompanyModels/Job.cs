namespace ManageEmployee.DataTransferObject.CompanyModels;

public class JobDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Status { get; set; }
    public string? Color { get; set; }
    public int Count { get; set; }
    public List<int> StatusIds { get; set; } = new List<int>();
}
