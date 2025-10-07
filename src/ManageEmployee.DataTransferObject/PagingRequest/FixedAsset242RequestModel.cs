namespace ManageEmployee.DataTransferObject.PagingRequest;

public class FixedAsset242RequestModel : PagingRequestModel
{
    public int Use { get; set; } = 1;// 0 het khau hao, 1 con khau hao
    public string Type { get; set; } = "PB";//  "PB"  ccdc, "KH": tai san co dinh

}