namespace ManageEmployee.DataTransferObject.Reports;

public class FinalStandardToLedgerModel
{
    public bool IsNotUpdate { get; set; }
    public List<FinalStandardDetailModel>? listData { get; set; }
}
