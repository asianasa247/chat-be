namespace ManageEmployee.DataTransferObject;

public class DeskFloorModel
{
    public int Id { get; set; }
    public int FloorId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public bool IsDesk { get; set; } = false;
    public bool IsFloor { get; set; } = false;
    public int NumberSeat { get; set; } = 0;
    public int Position { get; set; } = 0;
    public string? Description { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsChoose { get; set; } = false;
    public string? FloorName { get; set; }
    public int? Order { get; set; }
    public string? StatusName { get; set; }
    public int StatusId { get; set; }
}
