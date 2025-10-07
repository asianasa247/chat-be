using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.OrderEntities;

public class Payer
{
    public long Id { get; set; }

    [MaxLength(255)]
    public string? Code { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }
    [MaxLength(255)]
    public string? Address { get; set; }
    [MaxLength(255)]
    public string? Phone { get; set; }
    [MaxLength(255)]
    public string? Email { get; set; }
    [MaxLength(255)]
    public string? TaxCode { get; set; }
    [MaxLength(255)]
    public string? BankNumber { get; set; }
    [MaxLength(255)]
    public string? BankName { get; set; }
    [MaxLength(255)]
    public string? IdentityNumber { get; set; }
    [MaxLength(255)]
    public string? Product { get; set; }

    public int PayerType { get; set; } = 1; // PayerType = 1 -> Ca Nhan , PayerType = 2 -> To chuc
}
