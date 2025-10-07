using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.UserEntites;

public class AccountPay
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(36)]
    public string? Account { get; set; }
    [StringLength(255)]
    public string? AccountName { get; set; }
    [StringLength(36)]
    public string? Detail1 { get; set; }
    [StringLength(255)]
    public string? DetailName1 { get; set; }
    [StringLength(36)]
    public string? Detail2 { get; set; }
    [StringLength(255)]
    public string? DetailName2 { get; set; }

}
