using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class YearSale
{
    public int Id { get; set; }
    public int Year { get; set; }
    [MaxLength(255)]
    public string? Note { get; set; }
}
