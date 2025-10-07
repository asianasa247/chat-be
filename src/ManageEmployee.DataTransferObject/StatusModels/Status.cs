namespace ManageEmployee.DataTransferObject.StatusModels;

public class StatusDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CompanyId { get; set; } = 0;
    public bool StatusDetect { get; set; } = false;
    public string? Color { get; set; }
    public int Count { get; set; }
    public List<int> JobIds { get; set; } = new List<int>();
}
