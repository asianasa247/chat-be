namespace ManageEmployee.DataTransferObject.UserModels;

public class ThongKeTongQuat
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int SoLuong { get; set; }
    public bool isBold { get; set; }
    public List<ThongKeTongQuat>? listChildren { get; set; }
}
