using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ManageEmployee.Entities.BaseEntities;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities;

public class Slider : BaseEntity
{
    public int Id { get; set; }
    public LanguageEnum Type { get; set; }

    [StringLength(36)]
    public string? Name { get; set; }

    public string? Img { get; set; }

    /// <summary>
    /// Ảnh cho mobile. Không map DB để tránh lỗi khi DB không có cột.
    /// Response sẽ mirror từ Img ở tầng service.
    /// </summary>
    [NotMapped]
    public string? ImgMobile { get; set; }

    public int AdsensePosition { get; set; }
    public bool IsSizeImage { get; set; }
    public bool IsVideo { get; set; }
}
