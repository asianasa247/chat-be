using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.DataTransferObject.CategoryModels;

public class CategoryModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Type { get; set; } = 0;// CategoryEnum
    public int TypeView { get; set; } = 0;// CategoryEnum
    public string? Note { get; set; }
    public bool IsDeleted { get; set; } = false;

    public int? NumberItem { get; set; }      // số thứ tự
    public int? ProductCount { get; set; }    // NEW: số sản phẩm

    public bool IsPublish { get; set; } = false;
    public string? Icon { get; set; }
    public string? Image { get; set; }
    public string? CodeParent { get; set; }
    public string? NameEnglish { get; set; }
    public string? NameKorea { get; set; }
    public bool IsEnableDelete { get; set; } = false;
    public bool IsShowWeb { get; set; } = false;
    public List<UserTaskFileModel>? FileLink { get; set; }
    public double? TotalAmountBuy { get; set; }
    public bool IsSizeImage { get; set; }
    public string TypeMenu { get; set; }
}
