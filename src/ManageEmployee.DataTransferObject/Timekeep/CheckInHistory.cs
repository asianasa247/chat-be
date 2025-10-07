namespace ManageEmployee.DataTransferObject.Timekeep;

public class CheckInHistory
{
    public DateTime? TimeIn { get; set; }
    public int SymbolId { get; set; }
    public string? SymbolCode { get; set; }
}
