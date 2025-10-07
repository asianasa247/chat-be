using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities.ProcedureEntities;

public class ProcedureLog : BaseEntityCommon
{
    public int Id { get; set; }
    public int ProcedureId { get; set; }
    public string? ProcedureCode { get; set; }
    public int ProcedureStatusId { get; set; }
    public int UserId { get; set; }
    public int NotAcceptCount { get; set; }

    public string? UserIds { get; set; }
    public string? RoleIds { get; set; }
    public string? NotificationContent { get; set; }
    public bool IsSendNotification { get; set; }
}
