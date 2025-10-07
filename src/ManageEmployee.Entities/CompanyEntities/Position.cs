namespace ManageEmployee.Entities.CompanyEntities;

public class Position
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public int? Order { get; set; }
    public bool isDelete { get; set; }
}