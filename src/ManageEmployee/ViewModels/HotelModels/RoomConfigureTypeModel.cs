using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.ViewModels.HotelModels;

public class RoomConfigureTypeModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? NameVn { get; set; }
    public string? NameKo { get; set; }
    public string? NameEn { get; set; }
    public RoomConfigureTypeEnum? Type { get; set; }
    public List<RoomConfigureModel>? Items { get; set; }
}
public class RoomConfigureTypePagingModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? NameVn { get; set; }
    public string? NameKo { get; set; }
    public string? NameEn { get; set; }
}


