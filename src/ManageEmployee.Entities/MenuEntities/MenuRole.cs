namespace ManageEmployee.Entities.MenuEntities;

public class MenuRole
{
    public int Id { get; set; }
    public int? MenuId { get; set; }
    public int? UserRoleId { get; set; }
    public string? MenuCode { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool View { get; set; }
}
