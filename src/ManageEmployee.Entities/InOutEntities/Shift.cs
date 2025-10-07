using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.InOutEntities;

public class Shift : BaseEntityCommon
{
    public int Id { get; set; }
    public int SymbolId { get; set; }
    public TimeSpan TimeIn { get; set; }
    public TimeSpan TimeOut { get; set; }
}