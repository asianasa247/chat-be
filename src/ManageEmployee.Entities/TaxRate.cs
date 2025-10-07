using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class TaxRate
{
    public long Id { get; set; }

    [MaxLength(255)]
    public string? Code { get; set; }

    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? DebitCodeName { get; set; }

    [MaxLength(255)]
    public string? CreditCodeName { get; set; }

    public double Percent { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public int Type { get; set; }// 0 : dau vao, 1 : dau ra

    public int Order { get; set; }

    public string? DebitCode { get; set; }

    public string? CreditCode { get; set; }
    public string? DebitFirstCode { get; set; }

    public string? CreditFirstCode { get; set; }
    public string? DebitSecondCode { get; set; }

    public string? CreditSecondCode { get; set; }
}