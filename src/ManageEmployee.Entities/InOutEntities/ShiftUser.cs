using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class ShiftUser : BaseEntityCommon
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Note { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
}
