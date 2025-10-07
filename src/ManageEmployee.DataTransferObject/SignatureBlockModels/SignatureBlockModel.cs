using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.SignatureBlockModels;

public class SignatureBlockModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public FileDetailModel? File { get; set; }
    public List<int>? UserIds { get; set; }
    public string? Note { get; set; }
}
