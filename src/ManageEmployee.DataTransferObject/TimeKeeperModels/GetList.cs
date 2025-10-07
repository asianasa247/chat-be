using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;
public class GetList : BaseEntity
{
    public string FullName { get; set; }
    public bool isDelete { get; set; }
    public string Code { get; set; }
    public string TargetName { get; set; }
    public int TargetId { get; set; }
    public int TypeOfWork { get; set; }
    public int UserId { get; set; }
    public string DepartmentName { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
    public int TimeKeepSymbolId { get; set; }
    public int TimeKeepId { get; set; }
    public string Timekeep { get; set; }
    public DateTime? DateTimeKeep { get; set; }
    public string TargetCode { get; set; }
}