using ManageEmployee.Entities.BaseEntities;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class GetListReport : BaseEntity
{
    public string FullName { get; set; }
    public string Code { get; set; }
    public string DepartmentName { get; set; }
    public int UserId { get; set; }
    public List<TimeKeep> TypeKeepList { get; set; }
}