using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.DocumentModels;

public class DocumentType2ViewModel
{
    public int Id { get; set; }
    public int? BranchId { get; set; }
    public int? DocumentId { get; set; }
    public string? DocumentName { get; set; }
    public string? TextSymbol { get; set; }
    public DateTime? DateText { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? DraftarId { get; set; }
    public string? DraftarName { get; set; }
    public string? Content { get; set; }
    public int? SignerTextId { get; set; }
    public string? SignerTextName { get; set; }
    public string? Recipient { get; set; }
    public string? FileUrl { get; set; }
    public IFormFile? File { get; set; }
}
