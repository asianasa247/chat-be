using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities;

public class Description
{
    public long Id { get; set; }

    [MaxLength(255)]
    public string? DebitCode { get; set; }

    [MaxLength(255)]
    public string? CreditCode { get; set; }

    [MaxLength(255)]
    public string? DebitDetailCodeFirst { get; set; }

    [MaxLength(255)]
    public string? DebitDetailCodeSecond { get; set; }

    [MaxLength(255)]
    public string? CreditDetailCodeFirst { get; set; }

    [MaxLength(255)]
    public string? CreditDetailCodeSecond { get; set; }

    [MaxLength(500)]
    public string? Name { get; set; }
    public string DocumentCode { get; set; }
}