using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class InvoiceDeclaration : BaseEntity
{
    public int Id { get; set; }
    [StringLength(255)]
    public string? Name { get; set; }
    [StringLength(255)]
    public string? TemplateSymbol { get; set; }
    [StringLength(255)]
    public string? InvoiceSymbol { get; set; }
    public int? FromOpening { get; set; }
    public int? ToOpening { get; set; }
    public int? FromArising { get; set; }
    public int? ToArising { get; set; }
    [StringLength(255)]
    public string? Note { get; set; }

    public int? FromUsed { get; set; }
    public int? ToUsed { get; set; }
    public int? UsedNumber { get; set; }
    public int? DeleteNumber { get; set; }
    [StringLength(255)]
    public string? DeleteNumberItem { get; set; }
    public int Month { get; set; }

}
