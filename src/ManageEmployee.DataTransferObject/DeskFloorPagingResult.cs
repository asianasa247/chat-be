namespace ManageEmployee.DataTransferObject;

public class DeskFLoorPagingResult
{
    public int pageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public IEnumerable<DeskFloorModel> DeskFloors { get; set; } = new List<DeskFloorModel>();
}
