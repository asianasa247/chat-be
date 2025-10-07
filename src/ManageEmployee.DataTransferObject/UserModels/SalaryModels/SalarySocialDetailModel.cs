using ManageEmployee.DataTransferObject.V2;

namespace ManageEmployee.DataTransferObject.UserModels.SalaryModels;

public class SalarySocialDetailModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public CommonModel? Debit { get; set; }
    public CommonModel? DebitFirst { get; set; }
    public CommonModel? DebitSecond { get; set; }
    public CommonModel? Credit { get; set; }
    public CommonModel? CreditFirst { get; set; }
    public CommonModel? CreditSecond { get; set; }
    public int Order { get; set; }
    public double ValueCompany { get; set; }
    public double ValueUser { get; set; }

}
