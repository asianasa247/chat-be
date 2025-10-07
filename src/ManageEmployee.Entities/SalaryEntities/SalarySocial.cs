namespace ManageEmployee.Entities.SalaryEntities;

public class SalarySocial
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? AccountDebit { get; set; }
    public string? DetailDebit1 { get; set; }
    public string? DetailDebit2 { get; set; }
    public string? AccountCredit { get; set; }
    public string? DetailCredit1 { get; set; }
    public string? DetailCredit2 { get; set; }
    public int Order { get; set; }
    public double ValueCompany { get; set; }
    public double ValueUser { get; set; }

}
