using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.CategoryEntities;

public class Category
{
    public Category()
    {
        CategoryStatusWebPeriods = new HashSet<CategoryStatusWebPeriod>();
    }
    public int Id { get; set; }
    [StringLength(255)]
    public string Code { get; set; }
    [StringLength(500)]
    public string Name { get; set; }
    public int Type { get; set; } = 0;// CategoryEnum
    public int TypeView { get; set; } = 0;// CategoryEnum
    [StringLength(500)]
    public string Note { get; set; }
    public bool IsDeleted { get; set; } = false;

    public int? NumberItem { get; set; }       // số thứ tự (đang dùng cho menu)
    public int? ProductCount { get; set; }     // NEW: số sản phẩm

    public bool IsPublish { get; set; } = false;
    [StringLength(200)]
    public string Icon { get; set; }
    [StringLength(1000)]
    public string Image { get; set; }
    [StringLength(255)]
    public string CodeParent { get; set; }
    [StringLength(500)]
    public string NameEnglish { get; set; }
    [StringLength(500)]
    public string NameKorea { get; set; }
    public bool IsEnableDelete { get; set; } = false;
    public bool IsShowWeb { get; set; } = false;
    public double? TotalAmountBuy { get; set; }
    public ICollection<CategoryStatusWebPeriod> CategoryStatusWebPeriods { get; set; }
    public bool IsSizeImage { get; set; }
    public string TypeMenu { get; set; }
}
