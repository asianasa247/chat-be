using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.Entities;

public class SignatureBlock : BaseEntityCommon
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? FileStr { get; set; }
    public string? UserIdStr { get; set; }
    public string? Note { get; set; }
    public string? Notification { get; set; }
    public bool IsFinished { get; set; }
    public string? UserIdRefused { get; set; }
    public string? UserIdAccepted { get; set; }
}