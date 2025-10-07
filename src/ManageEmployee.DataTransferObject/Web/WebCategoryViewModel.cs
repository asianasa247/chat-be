namespace ManageEmployee.DataTransferObject.Web;

public class WebCategoryViewModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Type { get; set; } = 0;// CategoryEnum
    public int TypeView { get; set; } = 0;// CategoryEnum
    public string? Note { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int? NumberItem { get; set; }
    public bool isPublish { get; set; } = false;
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public string? CodeParent { get; set; }
    public string? NameEnglish { get; set; }
    public string? NameKorea { get; set; }
    public bool isEnableDelete { get; set; } = false;
    public bool IsShowWeb { get; set; } = false;
    public List<WebCategoryViewModel> Childrens { get; set; } = new List<WebCategoryViewModel>();
    public bool IsSizeImage { get; set; }
    public string TypeMenu { get; set; }
}
