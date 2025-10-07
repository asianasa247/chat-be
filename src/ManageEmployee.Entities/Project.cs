namespace ManageEmployee.Entities;

public class Project
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool Status { get; set; }    
    public DateTime CreateAt { get; set; } = DateTime.Now;
    public bool IsDeleted { get; set; } = false;
}
