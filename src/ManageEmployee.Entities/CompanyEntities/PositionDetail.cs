namespace ManageEmployee.Entities.CompanyEntities;

public class PositionDetail
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int PositionId { get; set; }
    public int Order { get; set; } = 0;
    public bool isDelete { get; set; }
    public bool IsManager { get; set; }
}
