namespace ManageEmployee.DataTransferObject.InOut;

public class Report
{
    public string? FullName { get; set; }
    public string? Code { get; set; }
    public int UserId { get; set; }
    public string? DepartmentName { get; set; }
    public List<GetForReport>? Histories { get; set; }
    public int SymbolId { get; set; }

}
