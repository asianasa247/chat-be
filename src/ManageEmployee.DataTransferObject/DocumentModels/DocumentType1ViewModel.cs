using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject.DocumentModels;

public class DocumentType1ViewModel
{
    public int? Id { get; set; }
    public int? DocumentTypeId { get; set; }
    public int? BranchId { get; set; }
    public string? DocumentTypeName { get; set; }
    public DateTime? ToDate { get; set; }
    public string? UnitName { get; set; }
    public string? TextSymbol { get; set; }
    public DateTime? DateText { get; set; }
    public string? Content { get; set; }
    public string? Signer { get; set; }
    public int? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public int? ReceiverId { get; set; }
    public string? ReceiverName { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public IFormFile? File { get; set; }
}
