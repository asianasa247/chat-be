using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.UserEntites;

public class UserTask
{
    public int Id { get; set; }

    [StringLength(255)]
    public string? Name { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public int? UserCreated { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? ViewAll { get; set; }
    public int? ParentId { get; set; }// dự án cha
    public int? Status { get; set; }//TaskStatusEnum
    public bool? IsDeleted { get; set; }

    [StringLength(1000)]
    public string? FileLink { get; set; }

    public int Viewer { get; set; }
    public int? DepartmentId { get; set; }

    //
    public int TypeWorkId { get; set; } = 0;

    public string? TypeWorkName { get; set; }
    public double Point { get; set; }
    public bool isProject { get; set; }
    public int IsStatusForManager { get; set; }// status
    public int? UserManagerId { get; set; }
    public DateTime? ManagerUpdateDate { get; set; }
    public int? CustomerId { get; set; }

}