namespace ManageEmployee.Entities.ChartOfAccountEntities;

public class ChartOfAccountGroupLink
{
    public int Id { get; set; }
    public string? CodeChartOfAccountGroup { get; set; }
    public string? CodeChartOfAccount { get; set; }
    public int Year { get; set; }
}