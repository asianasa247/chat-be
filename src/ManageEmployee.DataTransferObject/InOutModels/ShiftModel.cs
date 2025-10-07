namespace ManageEmployee.DataTransferObject.InOutModels;

public class ShiftModel
{
    public int Id { get; set; }
    public int SymbolId { get; set; }
    public TimeSpan TimeIn { get; set; }
    public TimeSpan TimeOut { get; set; }
}
