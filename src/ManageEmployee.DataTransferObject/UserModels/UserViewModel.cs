using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.UserModels;

public class UserViewModel : PagingRequestModel
{
    public int? Warehouseid { get; set; }
    public int? Positionid { get; set; }
    public int? Departmentid { get; set; }
    public bool? RequestPassword { get; set; }
    public bool? Quit { get; set; }
    public GenderEnum Gender { get; set; }
    public DateTime? Birthday { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int Targetid { get; set; }
    public int? TypeOfWork { get; set; }
    public int? Month { get; set; }
    public int? Degreeid { get; set; }
    public int? Certificateid { get; set; }
    public List<int> Ids { get; set; } = new List<int>();
}