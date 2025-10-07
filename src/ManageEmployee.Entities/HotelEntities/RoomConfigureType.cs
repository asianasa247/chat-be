using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Entities.HotelEntities;

public class RoomConfigureType
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? NameVn { get; set; }
    public string? NameKo { get; set; }
    public string? NameEn { get; set; }
    public RoomConfigureTypeEnum? Type { get; set; }
    public DateTime? CreateAt { get; set; } = DateTime.Now;
    public DateTime? UpdateAt { get; set; } = DateTime.Now;
}
