using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.CarEntities;

public class CarLocation : BaseProcedureEntityCommon
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
}
