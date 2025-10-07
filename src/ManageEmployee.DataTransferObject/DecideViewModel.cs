using Microsoft.AspNetCore.Http;

namespace ManageEmployee.DataTransferObject;

public class DecideViewModel
{
    public int Id { get; set; }
    public int? Type { get; set; }
    public string? Code { get; set; }
    public int? EmployeesId { get; set; }
    public int? DecideTypeId { get; set; }
    public DateTime? Date { get; set; }
    public string? Description { get; set; }
    public string? Note { get; set; }
    public string? FileUrl { get; set; }
    public IFormFile? File { get; set; }
}
