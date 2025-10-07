using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.DataTransferObject.DocumentModels;

public class DocumentV2Model
{
    public int Id { get; set; }
    public int Stt { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool AllowDelete { get; set; }
    public bool Check { get; set; }
    public string? UserId { get; set; }
    public string? UserCode { get; set; }
    public string? UserFullName { get; set; }
    public string? Title { get; set; }
    public CommonModel? Debit { get; set; }
    public CommonModel? DebitDetailFirst { get; set; }
    public CommonModel? DebitDetailSecond { get; set; }
    public CommonModel? Credit { get; set; }
    public CommonModel? CreditDetailFirst { get; set; }
    public CommonModel? CreditDetailSecond { get; set; }
    public User? UserManager { get; set; }
}
