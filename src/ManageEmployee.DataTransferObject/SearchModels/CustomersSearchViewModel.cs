using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.SearchModels;

public class CustomersSearchViewModel : PagingRequestModel
{
    public int? JobId { get; set; }
    public int? StatusId { get; set; }
    public int? ExportExcel { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? EmployeeId { get; set; }
    public DateTime? Birthday { get; set; }
    public GenderEnum? Gender { get; set; }
    public int? FromAge { get; set; }
    public int? ToAge { get; set; }
    public int? Area { get; set; }//0: mien bac; 1:mien nam
    public int Type { get; set; } = 0;//  0 khách hàng 1 nhà cung cấp 2 web
    public int? CustomerClassficationId { get; set; }
    public int? ProvinceId { get; set; }


}
