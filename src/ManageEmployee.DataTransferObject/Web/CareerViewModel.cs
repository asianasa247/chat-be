using Common.Helpers;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.Web;

public class CareerViewModel
{
    public int? Id { get; set; }
    public string Title { get; set; }
    public CareerGroupType Group { get; set; }
    public string GroupName { get; set; }
    public string Location { get; set; }
    public string Salary { get; set; }
    public WorkingMethodType WorkingMethod { get; set; }
    public string WorkingMethodName { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public string Department { get; set; }
    public DateTime ExpiredApply { get; set; }
    public string Description { get; set; }
    public LanguageEnum Type { get; set; }
    public string ImageUrl { get; set; }

    public CareerViewModel() { }
    public CareerViewModel(Career career)
    {
        Id = career.Id;
        Title = career.Title;
        Group = career.Group;
        GroupName = career.Group.GetDescription();
        Location = career.Location;
        Salary = career.Salary;
        WorkingMethod = career.WorkingMethod;
        ImageUrl = career.ImageUrl;
        if (career.WorkingMethod == WorkingMethodType.Shift)
        {
            WorkingMethodName = career.WorkingMethod.GetDescription() + " " + career.StartTime + " - " + career.EndTime;
        }
        else
        {
            WorkingMethodName = career.WorkingMethod.GetDescription();
        }
        StartTime = career.StartTime;
        EndTime = career.EndTime;
        Department = career.Department;
        ExpiredApply = career.ExpiredApply;
        Description = career.Description;
        Type = career.Type;
    }
}
