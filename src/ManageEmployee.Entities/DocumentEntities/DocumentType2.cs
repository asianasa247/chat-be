using System.ComponentModel.DataAnnotations;
using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.DocumentEntities;

public class DocumentType2 : BaseEntity
{
    public int Id { get; set; }
    public int DocumentId { get; set; }
    [StringLength(36)]
    public string? DocumentName { get; set; }
    [StringLength(100)]
    public string? TextSymbol { get; set; }
    public DateTime? DateText { get; set; }
    public int? BranchId { get; set; }
    public int? DepartmentId { get; set; }
    [StringLength(36)]
    public string? DepartmentName { get; set; }
    public int? DraftarId { get; set; }
    [StringLength(36)]
    public string? DraftarName { get; set; }
    [StringLength(1000)]
    public string? Content { get; set; }
    public int? SignerTextId { get; set; }
    [StringLength(36)]
    public string? SignerTextName { get; set; }
    [StringLength(36)]
    public string? Recipient { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }

}
