namespace ManageEmployee.DataTransferObject.AriseModels;
public class TransferModelRequest
{
    public string? DocumentType { get; set; }
    public int Month { get; set; }
    public List<long>? LedgerIds { get; set; }
    public bool isDeleteData { get; set; }
    public int TypeData { get; set; }// 2: HT;3:NB; 1: Cả hai
    public int FromTypeData { get; set; }// 2: HT;3:NB; 1: Cả hai
}
