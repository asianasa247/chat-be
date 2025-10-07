namespace ManageEmployee.DataTransferObject.MenuModels;

public class MenuViewPagingModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? NameEN { get; set; }
    public string? NameKO { get; set; }
    public string? CodeParent { get; set; }
    public string? Note { get; set; }
    public string? Roles { get; set; }
    public int Order { get; set; }
    public bool IsParent { get; set; }
}
