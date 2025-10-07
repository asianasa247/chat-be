namespace ManageEmployee.DataTransferObject.PagingRequest;

public class GoodRequestModel
{
    public int Type { get; set; }// 0:Tang, 1: giam, 2: khong
    public int TypeMoney { get; set; }// 0:%, 1: tien
    public double? Percent { get; set; }
    public double? Cash { get; set; }
    public List<int>? listId { get; set; }
    public int UserCreated { get; set; } = 0;
}
