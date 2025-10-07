using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_LeavePagingModel
{
    public int Id { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? Name { get; set; }
    [Required]
    public DateTime Fromdt { get; set; }
    [Required]
    public DateTime Todt { get; set; }
    public bool IsLicensed { get; set; }
    public string? ProcedureStatusName { get; set; }
    public DateTime? CreateAt { get; set; }
    public string UserCreatedName { get; set; } = "";
    public string UserUpdatedName { get; set; } = "";
    public string? Reason { get; set; }
}
