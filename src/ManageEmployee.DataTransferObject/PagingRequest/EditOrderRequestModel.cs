namespace ManageEmployee.DataTransferObject.PagingRequest;

public class EditOrderRequestModel
{
    public int EditOrderStart { get; set; }
    public int EditOrderEnd { get; set; }
    public int OrderType { get; set; }
    public int EditValue { get; set; }
    public int IsInternal { get; set; }
}
