namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_ProcedureStatusViewModel
{
    public int Type { get; set; }//1: khoi tao, 2:ket thuc
    public string? Name { get; set; }
    public string? Note { get; set; }
    public List<int>? RoleIds { get; set; }
    public List<int>? UserIds { get; set; }
    public int Id { get; set; }
    public bool IsDeleted { get; set; }
}
