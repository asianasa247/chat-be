using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.DocumentEntities;

public class DocumentType1 : BaseEntity
{
    public int Id { get; set; }
    public int DocumentTypeId { get; set; }
    [StringLength(36)]
    public string? DocumentTypeName { get; set; }
    public DateTime? ToDate { get; set; }
    [StringLength(36)]
    public string? UnitName { get; set; }
    [StringLength(36)]
    public string? TextSymbol { get; set; }
    public DateTime? DateText { get; set; }
    [StringLength(1000)]
    public string? Content { get; set; }
    [StringLength(36)]
    public string? Signer { get; set; }
    public int? DepartmentId { get; set; }
    public int? BranchId { get; set; }
    [StringLength(36)]
    public string? DepartmentName { get; set; }
    public int? ReceiverId { get; set; }
    [StringLength(36)]
    public string? ReceiverName { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }

}
