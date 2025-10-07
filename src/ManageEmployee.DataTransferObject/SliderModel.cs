using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject;

public class SliderModel
{
    public int Id { get; set; }
    public LanguageEnum Type { get; set; }
    public string? Name { get; set; }

    /// <summary>
    /// Ảnh hiển thị web (desktop/tablet).
    /// </summary>
    public string? Img { get; set; }

    /// <summary>
    /// Ảnh hiển thị mobile. BE sẽ auto đồng bộ với Img.
    /// </summary>
    public string? ImgMobile { get; set; }

    public DateTime? CreateAt { get; set; }
    public int AdsensePosition { get; set; }
    public bool IsSizeImage { get; set; }
    public bool IsVideo { get; set; }
}
