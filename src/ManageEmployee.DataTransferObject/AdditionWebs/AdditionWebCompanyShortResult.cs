namespace ManageEmployee.DataTransferObject.AdditionWebs;

public class AdditionWebCompanyShortResult
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? MST { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
    public string? WebsiteName { get; set; }
    public DateTime SignDate { get; set; } = DateTime.Now;
    public string? FileLogo { get; set; }
    public string FullLogo { get; set; }
    public int BusinessType { get; set; }
}
