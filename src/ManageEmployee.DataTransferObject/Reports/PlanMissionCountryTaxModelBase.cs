namespace ManageEmployee.DataTransferObject.Reports;

public class PlanMissionCountryTaxModelBase
{
    public string? title { get; set; }
    public string? code { get; set; }
    public string? accountcode { get; set; }
    public double soconphainopdauky { get; set; }
    public double sopstk_phainop { get; set; }
    public double sopstk_danop { get; set; }
    public double luykedn_phainop { get; set; }
    public double luykedn_danop { get; set; }
    public double soconphainopcuoiky { get; set; }
    public int? rowtype { get; set; }
}
