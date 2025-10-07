namespace ManageEmployee.DataTransferObject.MenuModels;

public class MenuCheckRole
{
    public string? MenuCode { get; set; }
    public bool Add { get; set; }
    public bool Edit { get; set; }
    public bool Delete { get; set; }
    public bool View { get; set; }
    public string? Name { get; set; }
    public string? NameEN { get; set; }
    public string? NameKO { get; set; }
    public int Order { get; set; }
    public int ProcedureCount { get; set; }
}
