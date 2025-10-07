using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class Target
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Address { get; set; }
    public int ArmyNumber { get; set; }
    public int Present { get; set; }
    public string? NameContact { get; set; }
    public DateTime? DateInvoice { get; set; }
    public double UnitPrice { get; set; }
    public double Total { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Phone { get; set; }
    public string? IdentityCode { get; set; }
    public string? Note { get; set; }
    public bool Status { get; set; }
    public int Order { get; set; }

    public double LatitudePoint { get; set; }
    public double LongitudePoint { get; set; }
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    public double AllowedRadius { get; set; }
    public bool LocationValidationEnable { get; set; }
}