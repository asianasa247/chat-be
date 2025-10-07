namespace ManageEmployee.DataTransferObject.AriseModels;

public class AriseUpdateOrginalVoucherRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Type { get; set; }
    public int? Month { get; set; }
    public int IsInternal { get; set; }
}
