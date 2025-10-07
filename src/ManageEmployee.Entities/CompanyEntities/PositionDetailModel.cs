namespace ManageEmployee.Entities.CompanyEntities;

public class PositionDetailModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int PositionId { get; set; }
    public string? PositionName { get; set; }
    public bool isDelete { get; set; }
    public int? Order { get; set; }

}
