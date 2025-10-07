namespace ManageEmployee.Entities;

public class Major
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string Note { get; set; } = "";
    public bool isDelete { get; set; }
}