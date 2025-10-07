using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class Surcharge
{
    public int Id { get; set; }
    [StringLength(36)]
    public string? Code { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public decimal Value { get; set; }
    [StringLength(36)]
    public string Type { get; set; } = "PHANTRAM";//PHANTRAM; VND
    [StringLength(255)]
    public string? Note { get; set; }
}
