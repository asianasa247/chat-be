namespace ManageEmployee.DataTransferObject.PagingRequest;

public class PetrolConsumptionReportRequestModel
{
    public DateTime FromAt { get; set; }
    public DateTime ToAt { get; set; }
    public int CarId { get; set; }
}
